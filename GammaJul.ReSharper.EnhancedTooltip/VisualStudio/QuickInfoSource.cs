using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
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
using JetBrains.UI.Avalon.Controls;
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
		
		private enum HighlighterType {
			Identifier,
			Issue,
			Other
		}

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

					var identifierPresenters = new List<UIElement>();
					var issuePresenters = new List<UIElement>();
					var otherPresenters = new List<UIElement>();

					foreach (Vs10Highlighter highlighter in highlighters) {
						var pair = TryPresentHighlighter(highlighter, documentMarkup);
						UIElement presenter = pair.First;
						if (presenter == null)
							continue;

						switch (pair.Second) {
							case HighlighterType.Identifier:
								identifierPresenters.Add(presenter);
								break;
							case HighlighterType.Issue:
								issuePresenters.Add(presenter);
								break;
							default:
								otherPresenters.Add(presenter);
								break;
						}
					}
					
					AddPresenters(identifierPresenters, "Id", "Ids", quickInfoContent);
					AddPresenters(issuePresenters, "Issue", "Issues", quickInfoContent);
					AddPresenters(otherPresenters, "Other", "Other", quickInfoContent);
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
		private IDocumentMarkup TryGetDocumentMarkup() {
			VSIVsTextBuffer bufferAdapter = _editorAdaptersFactoryService.GetBufferAdapter(_textBuffer);
			if (bufferAdapter == null)
				return null;

			IDocument document = new JetIVsTextBuffer(bufferAdapter).JetDocument.Value;
			if (document == null)
				return null;
			
			IDocumentMarkup documentMarkup = DocumentMarkupManagerBase.TryGetMarkupModel(document);
			// ReSharper disable once SuspiciousTypeConversion.Global
			return documentMarkup is IVsDocumentMarkup ? documentMarkup : null;
		}

		private static Pair<UIElement, HighlighterType> TryPresentHighlighter([NotNull] IHighlighter highlighter, [NotNull] IDocumentMarkup documentMarkup) {
			if (highlighter.Attributes.Effect.Type == EffectType.GUTTER_MARK)
				return Pair.Of((UIElement) null, HighlighterType.Other);

			Pair<TooltipContent, HighlighterType> pair = TryGetTooltipContent(highlighter, documentMarkup);
			TooltipContent tooltipContent = pair.First;
			HighlighterType highlighterType = pair.Second;
			if (tooltipContent == null || tooltipContent.MainText == null || tooltipContent.MainText.IsEmpty)
				return Pair.Of((UIElement) null, highlighterType);

			var dockPanel = PresentTooltipContent(tooltipContent);
			return Pair.Of(dockPanel, highlighterType);
		}

		[NotNull]
		private static UIElement PresentTooltipContent([NotNull] TooltipContent tooltipContent) {
			var mainTextPanel = new DockPanel { Margin = new Thickness(3, 2, 3, 2) };

			if (tooltipContent.Icon != null) {
				var image = new ThemedIconViewImage(tooltipContent.Icon) {
					Margin = new Thickness(0, 0, 4, 0),
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left
				};
				DockPanel.SetDock(image, Dock.Left);
				mainTextPanel.Children.Add(image);
			}

			mainTextPanel.Children.Add(CreateRichTextPresenter(tooltipContent.MainText, false));
			if (tooltipContent.DescriptionText.IsNullOrEmpty())
				return mainTextPanel;

			return new StackPanel {
				Orientation = Orientation.Vertical,
				Children = {
					mainTextPanel,
					CreateRichTextPresenter(tooltipContent.DescriptionText, true)
				}
			};
		}

		private static Pair<TooltipContent, HighlighterType> TryGetTooltipContent([NotNull] IHighlighter highlighter, [NotNull] IDocumentMarkup documentMarkup) {
			var highlighting = highlighter.UserData as IHighlighting;
			if (highlighting == null)
				return Pair.Of(TryCreateTooltipContent(highlighter.RichTextToolTip, null), HighlighterType.Other);

			Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, documentMarkup.Document);
			IconId severityIcon = TryGetSeverityIcon(severity);
			if (severityIcon != null)
				return Pair.Of(TryCreateTooltipContent(highlighter.RichTextToolTip, severityIcon), HighlighterType.Issue);

			var highlighterType = HighlighterType.Other;
			PsiLanguageType languageType = TryGetIdentifierLanguage(highlighting);
			TooltipContent tooltipContent = null;
			if (languageType != null) {
				highlighterType = HighlighterType.Identifier;
				tooltipContent = TryGetIdentifierTooltipContent(highlighter, languageType);
			}

			return Pair.Of(tooltipContent ?? TryCreateTooltipContent(highlighter.RichTextToolTip, null), highlighterType);
		}

		[CanBeNull]
		private static TooltipContent TryGetIdentifierTooltipContent([NotNull] IHighlighter highlighter, [NotNull] PsiLanguageType languageType) {
			var solutionManager = Shell.Instance.TryGetComponent<VSSolutionManager>();
			if (solutionManager == null)
				return null;

			ISolution solution = solutionManager.CurrentSolution;
			if (solution == null)
				return null;

			var provider = solution.GetComponent<IdentifierTooltipContentProvider>();
			return provider.TryGetTooltipContent(highlighter, languageType);
		}

		[CanBeNull]
		private static TooltipContent TryCreateTooltipContent([CanBeNull] RichTextBlock richTextTooltip, [CanBeNull] IconId icon) {
			if (richTextTooltip == null)
				return null;

			RichText richText = richTextTooltip.RichText;
			if (richText.IsEmpty)
				return null;

			return new TooltipContent {
				MainText = richText,
				Icon = icon
			};
		}

		private TextRange GetCurrentTextRange([NotNull] IIntellisenseSession session) {
			ITextSnapshot currentSnapshot = _textBuffer.CurrentSnapshot;
			SnapshotPoint currentPoint = session.GetTriggerPoint(_textBuffer).GetPoint(currentSnapshot);
			//var currentSpan = new SnapshotSpan(currentPoint, currentPoint.Position < currentSnapshot.Length ? 1 : 0);
			var currentSpan = new SnapshotSpan(currentPoint, 0);
			return new TextRange(currentSpan.Start, currentSpan.End);
		}

		[CanBeNull]
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

		private static void AddPresenters([NotNull] IReadOnlyList<UIElement> presenters, [NotNull] string singularHeader,
			[NotNull] string pluralHeader, [NotNull] ICollection<object> quickInfoContent) {
			switch (presenters.Count) {
				case 0:
					return;

				case 1:
					quickInfoContent.Add(CreaterHeaderedContentControl(singularHeader, presenters[0]));
					break;

				default:
					var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
					foreach (UIElement presenter in presenters)
						stackPanel.Children.Add(presenter);
					quickInfoContent.Add(CreaterHeaderedContentControl(pluralHeader, stackPanel));
					break;
			}
		}

		[NotNull]
		private static HeaderedContentControl CreaterHeaderedContentControl([CanBeNull] object header, [CanBeNull] UIElement content) {
			return new HeaderedContentControl {
				Style = UIResources.Instance.HeaderedContentControlStyle,
				Header = header,
				Content = content
			};
		}

		[NotNull]
		private static UIElement CreateRichTextPresenter([CanBeNull] RichText richText, bool isAutoContrasted) {
			var richTextPresenter = new RichTextPresenter(richText) {
				IsAutoContrasted = isAutoContrasted,
				TextWrapping = TextWrapping.Wrap
			};
			
			TextOptions.SetTextFormattingMode(richTextPresenter, TextFormattingMode.Ideal);
			TextOptions.SetTextRenderingMode(richTextPresenter, TextRenderingMode.ClearType);
			
			object foreground = richTextPresenter.TryFindResource(ThemeColor.TooltipForeground);
			if (foreground is Color)
				richTextPresenter.Foreground = new SolidColorBrush((Color) foreground);

			return richTextPresenter;
		}
		
		[CanBeNull]
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