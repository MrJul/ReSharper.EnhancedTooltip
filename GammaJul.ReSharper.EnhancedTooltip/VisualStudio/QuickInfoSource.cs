using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Avalon.Controls;
using JetBrains.UI.RichText;
using JetBrains.Util;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using VSIVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;
#if RS90
using JetBrains.Platform.VisualStudio.SinceVs10.Interop.Shim.IDE;
using JetBrains.Platform.VisualStudio.SinceVs10.TextControl.Markup;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.VsIntegration.ProjectDocuments;
using JetIVsTextBuffer = JetBrains.VsIntegration.Interop.Shim.TextManager.Documents.IVsTextBuffer;
#elif RS82
using JetBrains.VsIntegration.DevTen.Interop.Shim;
using JetBrains.VsIntegration.DevTen.Markup;
using JetBrains.VsIntegration.ProjectModel;
using JetIVsTextBuffer = JetBrains.VsIntegration.Interop.Shim.TextManager.IVsTextBuffer;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class QuickInfoSource : IQuickInfoSource {

		private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
		private readonly ITextBuffer _textBuffer;
		
		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
			applicableToSpan = null;

			if (session == null || quickInfoContent == null || quickInfoContent.IsReadOnly)
				return;
			
			IDocumentMarkup documentMarkup = TryGetDocumentMarkup();
			if (documentMarkup == null)
				return;

			// If this fail, it means the extension is disabled and none of are components are available.
			var tooltipFontProvider = Shell.Instance.TryGetComponent<TooltipFormattingProvider>();
			if (tooltipFontProvider == null)
				return;

			ITextSnapshot textSnapshot = _textBuffer.CurrentSnapshot;
			TextRange textRange = GetCurrentTextRange(session, textSnapshot);
			IShellLocks shellLocks = documentMarkup.Context.Locks;
			Span? finalSpan = null;

			Action getEnhancedTooltips = () => {
				using (shellLocks.UsingReadLock()) {
					
					var presenter = new MultipleTooltipContentPresenter(tooltipFontProvider.GetTooltipFormatting());

					List<Vs10Highlighter> highlighters = documentMarkup.GetHighlightersOver(textRange).OfType<Vs10Highlighter>().ToList();
					foreach (Vs10Highlighter highlighter in highlighters) {
						if (presenter.TryAddContent(TryGetTooltipContent(highlighter, documentMarkup)))
							finalSpan = highlighter.Range.ToSpan().Union(finalSpan);
					}

					var nonReSharperContents = new List<object>();
					foreach (object content in quickInfoContent) {
						// ignore existing R# elements
						if (!(content is RichTextPresenter))
							nonReSharperContents.Add(content);
					}

					quickInfoContent.Clear();
					quickInfoContent.AddRange(presenter.PresentContents());
					quickInfoContent.AddRange(nonReSharperContents);

				}
			};

			if (shellLocks.ReentrancyGuard.TryExecute("GetEnhancedTooltips", getEnhancedTooltips) && finalSpan != null)
				applicableToSpan = textSnapshot.CreateTrackingSpan(finalSpan.Value, SpanTrackingMode.EdgeInclusive);
		}

		[CanBeNull]
		[Pure]
		private IDocumentMarkup TryGetDocumentMarkup() {
			VSIVsTextBuffer bufferAdapter = _editorAdaptersFactoryService.GetBufferAdapter(_textBuffer);
			if (bufferAdapter == null)
				return null;

			IDocument document = new JetIVsTextBuffer(bufferAdapter).JetDocument.Value;
			if (document == null)
				return null;
			
			IDocumentMarkup documentMarkup = DocumentMarkupManagerBase.TryGetMarkupModel(document);
			if (documentMarkup == null || !documentMarkup.GetType().Name.StartsWith("Vs", StringComparison.Ordinal))
				return null;
			
			return documentMarkup;
		}

		[CanBeNull]
		[Pure]
		private static ITooltipContent TryGetTooltipContent([NotNull] IHighlighter highlighter, [NotNull] IDocumentMarkup documentMarkup) {
			if (highlighter.Attributes.Effect.Type == EffectType.GUTTER_MARK)
				return null;

			var highlighting = highlighter.UserData as IHighlighting;
			if (highlighting != null) {

				ISolution solution = TryGetCurrentSolution();

				IDocument document = documentMarkup.Document;
				IContextBoundSettingsStore settings = document.GetSettings();

				Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, document, solution);
				IssueTooltipContent issueContent = TryCreateIssueContent(highlighting, highlighter.RichTextToolTip, severity, settings, solution);
				if (issueContent != null)
					return issueContent;

				PsiLanguageType languageType = TryGetIdentifierLanguage(highlighting);
				if (languageType != null) {
					if (solution != null) {
						IdentifierTooltipContent identifierContent = TryGetIdentifierTooltipContent(highlighter, languageType, solution, settings);
						if (identifierContent != null)
							return identifierContent;
					}
				}
			}

			return TryCreateMiscContent(highlighter.RichTextToolTip);
		}

		[CanBeNull]
		private static ISolution TryGetCurrentSolution() {
			var solutionManager = Shell.Instance.TryGetComponent<VSSolutionManager>();
			return solutionManager != null ? solutionManager.CurrentSolution : null;
		}

		[CanBeNull]
		[Pure]
		private static IdentifierTooltipContent TryGetIdentifierTooltipContent([NotNull] IHighlighter highlighter, [NotNull] PsiLanguageType languageType,
			[NotNull] ISolution solution, [NotNull] IContextBoundSettingsStore settings) {
			var contentProvider = solution.GetComponent<IdentifierTooltipContentProvider>();
			return contentProvider.TryGetIdentifierContent(highlighter, languageType, settings);
		}

		[CanBeNull]
		[Pure]
		private static IssueTooltipContent TryCreateIssueContent([NotNull] IHighlighting highlighting, [CanBeNull] RichTextBlock textBlock,
			Severity severity, [NotNull] IContextBoundSettingsStore settings, [CanBeNull] ISolution solution) {

			if (textBlock == null || !severity.IsIssue())
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			RichText enhancedText = TryEnhanceHighlighting(highlighting, settings, solution);
			if (!enhancedText.IsNullOrEmpty())
				text = enhancedText;

			var issueContent = new IssueTooltipContent { Text = text };
			if (settings.GetValue((IssueTooltipSettings s) => s.ShowIcon))
				issueContent.Icon = severity.TryGetIcon();
			return issueContent;
		}

		[CanBeNull]
		private static RichText TryEnhanceHighlighting([NotNull] IHighlighting highlighting, [NotNull] IContextBoundSettingsStore settings,
			[CanBeNull] ISolution solution) {
			if (solution == null)
				return null;
			
			var highlightingEnhancerManager = solution.TryGetComponent<HighlightingEnhancerManager>();
			if (highlightingEnhancerManager == null)
				return null;

			return highlightingEnhancerManager.TryEnhance(highlighting, settings);
		}

		[CanBeNull]
		[Pure]
		private static MiscTooltipContent TryCreateMiscContent([CanBeNull] RichTextBlock textBlock) {
			if (textBlock == null)
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			return new MiscTooltipContent { Text = text };
		}

		[Pure]
		private TextRange GetCurrentTextRange([NotNull] IIntellisenseSession session, [NotNull] ITextSnapshot textSnapshot) {
			SnapshotPoint currentPoint = session.GetTriggerPoint(_textBuffer).GetPoint(textSnapshot);
			var currentSpan = new SnapshotSpan(currentPoint, 0);
			return new TextRange(currentSpan.Start, currentSpan.End);
		}

		[CanBeNull]
		[Pure]
		private static PsiLanguageType TryGetIdentifierLanguage([NotNull] IHighlighting highlighting) {
			var attribute = highlighting.GetType().GetCustomAttribute<DaemonTooltipProviderAttribute>();
			if (attribute == null || attribute.Type == null)
				return null;

			for (Type type = attribute.Type; type != typeof(object) && type != null; type = type.BaseType) {
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IdentifierTooltipProvider<>)) {
					Type langType = type.GetGenericArguments()[0];
					return Languages.Instance.All.FirstOrDefault(lang => lang.GetType() == langType);
				}
				
			}
			return null;
		}

		public void Dispose() {
		}

		public QuickInfoSource([NotNull] IVsEditorAdaptersFactoryService editorAdaptersFactoryService, [NotNull] ITextBuffer textBuffer) {
			_editorAdaptersFactoryService = editorAdaptersFactoryService;
			_textBuffer = textBuffer;
		}

	}

}