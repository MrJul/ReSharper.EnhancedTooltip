using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.Platform.VisualStudio.SinceVs10.Interop.Shim.IDE;
using JetBrains.Platform.VisualStudio.SinceVs10.TextControl.Markup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Avalon.Controls;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VsIntegration.ProjectDocuments;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using VSIVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;
using JetIVsTextBuffer = JetBrains.VsIntegration.Interop.Shim.TextManager.Documents.IVsTextBuffer;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class QuickInfoSource : IQuickInfoSource {

		[NotNull] private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
		[NotNull] private readonly ITextBuffer _textBuffer;
		
		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
			applicableToSpan = null;

			if (session == null || quickInfoContent == null || quickInfoContent.IsReadOnly)
				return;

			IDocumentMarkup documentMarkup = TryGetDocumentMarkup();
			if (documentMarkup == null)
				return;

			// If this fail, it means the extension is disabled and none of the components are available.
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
					IContextBoundSettingsStore settings = documentMarkup.Document.GetSettings();
					ISolution solution = TryGetCurrentSolution();

					bool hasIdentifierTooltipContent = false;
					if (solution != null) {
						DocumentRange documentRange = textRange.CreateDocumentRange(documentMarkup.Document);
						IdentifierTooltipContent[] contents = GetIdentifierTooltipContents(documentRange, solution, settings);
						foreach (IdentifierTooltipContent content in contents) {
							if (presenter.TryAddContent(content)) {
								finalSpan = content.TrackingRange.ToSpan();
								hasIdentifierTooltipContent = true;
							}
						}
					}

					List<Vs10Highlighter> highlighters = documentMarkup.GetHighlightersOver(textRange).OfType<Vs10Highlighter>().ToList();
					foreach (Vs10Highlighter highlighter in highlighters) {
						ITooltipContent[] contents = GetTooltipContents(highlighter, highlighter.Range, documentMarkup, solution, hasIdentifierTooltipContent);
						foreach (ITooltipContent content in contents) {
							if (presenter.TryAddContent(content))
								finalSpan = content.TrackingRange.ToSpan().Union(finalSpan);
						}
					}

					var nonReSharperContents = new List<object>();
					bool ignoredFirstTextBuffer = false;
					foreach (object content in quickInfoContent) {
						if (content == null)
							continue;

						// ignore existing R# elements
						if (content is RichTextPresenter)
							continue;

						if (hasIdentifierTooltipContent) {

							// ignore Roslyn identifier tooltip (for VS2015)
							if (content.GetType().FullName == "Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo.QuickInfoDisplayPanel")
								continue;

							// ignore the first VS text buffer (for VS2012 and VS2013)
							if (content is ITextBuffer && !ignoredFirstTextBuffer) {
								ignoredFirstTextBuffer = true;
								continue;
							}

						}
						
						nonReSharperContents.Add(content);
					}

					quickInfoContent.Clear();
					quickInfoContent.AddRange(presenter.PresentContents());

					if (nonReSharperContents.Count > 0) {
						var control = new HeaderedContentControl {
							Style = UIResources.Instance.HeaderedContentControlStyle,
							Focusable = false,
							Header = "VS",
							Content = new ItemsControl {
								Focusable = false,
								ItemsSource = nonReSharperContents
							}
						};
						quickInfoContent.Add(control);
					}

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

		[NotNull]
		private static IdentifierTooltipContent[] GetIdentifierTooltipContents(
			DocumentRange documentRange,
			[NotNull] ISolution solution,
			[NotNull] IContextBoundSettingsStore settings)
			=> solution
				.GetComponent<IdentifierTooltipContentProvider>()
				.GetIdentifierContents(documentRange, settings);

		[NotNull]
		[Pure]
		private static ITooltipContent[] GetTooltipContents([NotNull] IHighlighter highlighter, TextRange range,
			[NotNull] IDocumentMarkup documentMarkup, [CanBeNull] ISolution solution, bool skipIdentifierHighlighting) {

			if (highlighter.Attributes.Effect.Type == EffectType.GUTTER_MARK)
				return EmptyArray<ITooltipContent>.Instance;

			var highlighting = highlighter.UserData as IHighlighting;
			if (highlighting != null) {
				
				IDocument document = documentMarkup.Document;
				IContextBoundSettingsStore settings = document.GetSettings();

				Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, solution);
				IssueTooltipContent issueContent = TryCreateIssueContent(highlighting, range, highlighter.RichTextToolTip, severity, settings, solution);
				if (issueContent != null)
					return new ITooltipContent[] { issueContent };

				if (solution != null && IsIdentifierHighlighting(highlighting)) {
					if (skipIdentifierHighlighting)
						return EmptyArray<ITooltipContent>.Instance;
					IdentifierTooltipContent[] identifierContents = GetIdentifierTooltipContents(highlighter, solution, settings);
					// ReSharper disable once CoVariantArrayConversion
					return identifierContents;
				}
			}

			MiscTooltipContent miscContent = TryCreateMiscContent(highlighter.RichTextToolTip, range);
			if (miscContent != null)
				return new ITooltipContent[] { miscContent };

			return EmptyArray<ITooltipContent>.Instance;
		}

		[CanBeNull]
		private static ISolution TryGetCurrentSolution()
			=> Shell.Instance
				.TryGetComponent<VSSolutionManager>()
				?.CurrentSolution;

		[NotNull]
		[Pure]
		private static IdentifierTooltipContent[] GetIdentifierTooltipContents(
			[NotNull] IHighlighter highlighter,
			[NotNull] ISolution solution,
			[NotNull] IContextBoundSettingsStore settings)
			=> solution
				.GetComponent<IdentifierTooltipContentProvider>()
				.GetIdentifierContents(highlighter, settings);

		[CanBeNull]
		[Pure]
		private static IssueTooltipContent TryCreateIssueContent([NotNull] IHighlighting highlighting, TextRange trackingRange,
			[CanBeNull] RichTextBlock textBlock, Severity severity, [NotNull] IContextBoundSettingsStore settings, [CanBeNull] ISolution solution) {

			if (textBlock == null || !severity.IsIssue())
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			if (settings.GetValue((IssueTooltipSettings s) => s.ColorizeElementsInErrors)) {
				RichText enhancedText = TryEnhanceHighlighting(highlighting, settings, solution);
				if (!enhancedText.IsNullOrEmpty())
					text = enhancedText;
			}

			var issueContent = new IssueTooltipContent(text, trackingRange);
			if (settings.GetValue((IssueTooltipSettings s) => s.ShowIcon))
				issueContent.Icon = severity.TryGetIcon();
			return issueContent;
		}

		[CanBeNull]
		private static RichText TryEnhanceHighlighting(
			[NotNull] IHighlighting highlighting,
			[NotNull] IContextBoundSettingsStore settings,
			[CanBeNull] ISolution solution)
			=> solution
				?.TryGetComponent<HighlightingEnhancerManager>()
				?.TryEnhance(highlighting, settings);

		[CanBeNull]
		[Pure]
		private static MiscTooltipContent TryCreateMiscContent([CanBeNull] RichTextBlock textBlock, TextRange range) {
			if (textBlock == null)
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			return new MiscTooltipContent(text, range);
		}

		[Pure]
		private TextRange GetCurrentTextRange([NotNull] IIntellisenseSession session, [NotNull] ITextSnapshot textSnapshot) {
			SnapshotPoint currentPoint = session.GetTriggerPoint(_textBuffer).GetPoint(textSnapshot);
			var currentSpan = new SnapshotSpan(currentPoint, 0);
			return new TextRange(currentSpan.Start, currentSpan.End);
		}

		[Pure]
		private static bool IsIdentifierHighlighting([NotNull] IHighlighting highlighting) {
			var attribute = HighlightingSettingsManager.Instance.GetHighlightingAttribute(highlighting) as StaticSeverityHighlightingAttribute;
			return attribute?.GroupId == HighlightingGroupIds.IdentifierHighlightingsGroup;
		}

		public void Dispose() {
		}

		public QuickInfoSource([NotNull] IVsEditorAdaptersFactoryService editorAdaptersFactoryService, [NotNull] ITextBuffer textBuffer) {
			_editorAdaptersFactoryService = editorAdaptersFactoryService;
			_textBuffer = textBuffer;
		}

	}

}