using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.SolutionAnalysis.UI.Resources;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Components.Theming;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VsIntegration.DevTen.Markup;
using JetBrains.VsIntegration.Markup;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using VSIVsTextBuffer = Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer;
using JetIVsTextBuffer = JetBrains.VsIntegration.Interop.Shim.TextManager.IVsTextBuffer;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class QuickInfoSource : IQuickInfoSource {

		private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
		private readonly ITextBuffer _textBuffer;
		
		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
			applicableToSpan = null;

			IDocumentMarkup documentMarkup = TryGetDocumentMarkup();
			if (documentMarkup == null)
				return;

			var textRange = GetCurrentTextRange(session);
			IShellLocks shellLocks = documentMarkup.Context.Locks;

			quickInfoContent.Clear();

			Action getEnhancedTooltips = () => {
				using (shellLocks.UsingReadLock()) {
					
					List<Vs10Highlighter> highlighters = documentMarkup.GetHighlightersOver(textRange).OfType<Vs10Highlighter>().ToList();

					var identifierContents = new List<ITooltipContent>();
					var issueContents = new List<ITooltipContent>();
					var miscContents = new List<ITooltipContent>();

					foreach (Vs10Highlighter highlighter in highlighters) {
						ITooltipContent tooltipContent = TryGetTooltipContent(highlighter, documentMarkup);
						if (tooltipContent == null || tooltipContent.Text.IsNullOrEmpty())
							continue;

						if (tooltipContent is IdentifierContent)
							identifierContents.Add(tooltipContent);
						else if (tooltipContent is IssueContent)
							issueContents.Add(tooltipContent);
						else
							miscContents.Add(tooltipContent);
					}

					foreach (ITooltipContent identifierContent in identifierContents)
						quickInfoContent.Add(PresentTooltipContents("Id", new[] { identifierContent }));

					if (issueContents.Count > 0)
						quickInfoContent.Add(PresentTooltipContents(issueContents.Count == 1 ? "Issue" : "Issues", issueContents));

					foreach (ITooltipContent miscContent in miscContents)
						quickInfoContent.Add(PresentTooltipContents("Misc", new[] { miscContent }));

				}
			};

			if (!shellLocks.ReentrancyGuard.TryExecute("GetEnhancedTooltips", getEnhancedTooltips))
				return;

			/*var tooltips = new List<RichTextBlock>();
			Span? unionSpan = new Span?();
			bool hasErrorHighlighters = false;
			Action action = (Action) (() => {
				using (documentMarkup.Locks.UsingReadLock()) {
					foreach (Vs10Highlighter item_3 in Enumerable.ToList<Vs10Highlighter>(Enumerable.OfType<Vs10Highlighter>((IEnumerable) documentMarkup.GetHighlightersOver(textRange)))) {
						if (item_3.Attributes.Effect.Type != EffectType.GUTTER_MARK) {
							if (item_3.ErrorStripeAttributes.Kind == ErrorStripeKind.ERROR)
								hasErrorHighlighters = true;
							RichTextBlock local_2 = item_3.RichTextToolTip;
							if (local_2 != null && !string.IsNullOrEmpty(local_2.Text)) {
								tooltips.Add(local_2);
								Span local_3 = ConversionDevTen.ToSpan(item_3.Range);
								unionSpan = new Span?(!unionSpan.HasValue ? local_3 : Span.FromBounds(Math.Min(unionSpan.Value.Start, local_3.Start), Math.Max(unionSpan.Value.End, local_3.End)));
							}
						}
					}
				}
			});
			if (!documentMarkup.Locks.ReentrancyGuard.TryExecute("GetTooltips", action))
				return;
			if (unionSpan.HasValue) {
				if (!this.IsTypeScript() || hasErrorHighlighters) {
					quickInfoContent.Clear();
				}
				else {
					foreach (object obj in (IEnumerable<object>) quickInfoContent) {
						string str = obj is ITextBuffer ? ((ITextBuffer) obj).CurrentSnapshot.GetText().Trim() : obj.ToString().Trim();
						if (str.StartsWith("(") || str.IndexOf(' ') < 0)
							quickInfoContent.Remove(obj);
					}
				}
				foreach (RichTextBlock block in tooltips)
					quickInfoContent.Add((object) ToolTipUtil.CreateUIElement(block, (ThemeColor) null, (ThemeColor) null, true));
				applicableToSpan = currentSnapshot.CreateTrackingSpan(unionSpan.Value, SpanTrackingMode.EdgeInclusive);
			}
			else {
				if (!this.IsVsCSharpAutoPopupDisabled())
					return;
				foreach (ITextBuffer textBuffer in Enumerable.OfType<ITextBuffer>((IEnumerable) quickInfoContent)) {
					ITextSnapshot currentSnapshot2 = textBuffer.CurrentSnapshot;
					if (currentSnapshot2.Length >= "(dynamic".Length && currentSnapshot2.GetText(0, "(dynamic".Length) == "(dynamic") {
						quickInfoContent.Remove((object) textBuffer);
						break;
					}
				}
			}*/
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
			return documentMarkup is IVsDocumentMarkup ? documentMarkup : null;
		}

		[CanBeNull]
		[Pure]
		private static ITooltipContent TryGetTooltipContent([NotNull] IHighlighter highlighter, [NotNull] IDocumentMarkup documentMarkup) {
			if (highlighter.Attributes.Effect.Type == EffectType.GUTTER_MARK)
				return null;

			var highlighting = highlighter.UserData as IHighlighting;
			if (highlighting != null) {

				Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, documentMarkup.Document);
				IconId severityIcon = TryGetSeverityIcon(severity);
				if (severityIcon != null)
					return TryCreateIssueContent(highlighter.RichTextToolTip, severityIcon);

				PsiLanguageType languageType = TryGetIdentifierLanguage(highlighting);
				if (languageType != null)
					return TryGetIdentifierTooltipContent(highlighter, languageType);

			}
			
			return TryCreateMiscContent(highlighter.RichTextToolTip);
		}

		[CanBeNull]
		[Pure]
		private static IdentifierContent TryGetIdentifierTooltipContent([NotNull] IHighlighter highlighter, [NotNull] PsiLanguageType languageType) {
			var solutionManager = Shell.Instance.TryGetComponent<VSSolutionManager>();
			if (solutionManager == null)
				return null;

			ISolution solution = solutionManager.CurrentSolution;
			if (solution == null)
				return null;

			var provider = solution.GetComponent<IdentifierTooltipContentProvider>();
			return provider.TryGetIdentifierText(highlighter, languageType);
		}

		[CanBeNull]
		[Pure]
		private static IssueContent TryCreateIssueContent([CanBeNull] RichTextBlock textBlock, [CanBeNull] IconId icon) {
			if (textBlock == null)
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			return new IssueContent {
				Text = text,
				Icon = icon
			};
		}

		[CanBeNull]
		[Pure]
		private static MiscContent TryCreateMiscContent([CanBeNull] RichTextBlock textBlock) {
			if (textBlock == null)
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			return new MiscContent {
				Text = text
			};
		}

		[Pure]
		private TextRange GetCurrentTextRange([NotNull] IIntellisenseSession session) {
			ITextSnapshot currentSnapshot = _textBuffer.CurrentSnapshot;
			SnapshotPoint currentPoint = session.GetTriggerPoint(_textBuffer).GetPoint(currentSnapshot);
			//var currentSpan = new SnapshotSpan(currentPoint, currentPoint.Position < currentSnapshot.Length ? 1 : 0);
			var currentSpan = new SnapshotSpan(currentPoint, 0);
			return new TextRange(currentSpan.Start, currentSpan.End);
		}

		[CanBeNull]
		[Pure]
		private static IconId TryGetSeverityIcon(Severity severity) {
			switch (severity) {
				case Severity.HINT:
					return SolutionAnalysisThemedIcons.SolutionAnalysisHint.Id;
				case Severity.SUGGESTION:
					return SolutionAnalysisThemedIcons.SolutionAnalysisSuggestion.Id;
				case Severity.WARNING:
					return SolutionAnalysisThemedIcons.SolutionAnalysisWarning.Id;
				case Severity.ERROR:
					return SolutionAnalysisThemedIcons.SolutionAnalysisError.Id;
				default:
					return null;
			}
		}

		[NotNull]
		[Pure]
		private static HeaderedContentControl PresentTooltipContents([CanBeNull] object header, [NotNull] IEnumerable<ITooltipContent> tooltipContents) {
			var control = new HeaderedContentControl {
				Style = UIResources.Instance.HeaderedContentControlStyle,
				Focusable = false,
				Header = header,
				Content = new ItemsControl {
					Focusable = false,
					ItemsSource = tooltipContents
				}
			};

			object foreground = control.TryFindResource(ThemeColor.TooltipForeground);
			if (foreground is Color)
				control.Foreground = new SolidColorBrush((Color) foreground);
			return control;
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