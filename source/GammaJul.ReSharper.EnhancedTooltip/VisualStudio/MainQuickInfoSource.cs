using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.Platform.VisualStudio.SinceVs10.Interop.Shim.TextControl;
using JetBrains.Platform.VisualStudio.SinceVs10.TextControl.Markup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VsIntegration.ProjectDocuments;
using JetBrains.VsIntegration.TextControl;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class MainQuickInfoSource : QuickInfoSourceBase {

		protected override void AugmentQuickInfoSessionCore(
			IQuickInfoSession session,
			IList<object> quickInfoContent,
			IDocumentMarkup documentMarkup,
			TooltipFormattingProvider tooltipFormattingProvider,
			out ITrackingSpan applicableToSpan) {

			applicableToSpan = null;

			ITextSnapshot textSnapshot = TextBuffer.CurrentSnapshot;
			TextRange textRange = GetCurrentTextRange(session, textSnapshot);
			IShellLocks shellLocks = Shell.Instance.GetComponent<IShellLocks>();
			Span? finalSpan = null;

			void GetEnhancedTooltips() {
				using (shellLocks.UsingReadLock()) {

					IDocument document = documentMarkup.Document;
					var presenter = new MultipleTooltipContentPresenter(tooltipFormattingProvider.GetTooltipFormatting(), document);
					IContextBoundSettingsStore settings = document.GetSettings();
					ISolution solution = TryGetCurrentSolution();
					bool hasIdentifierTooltipContent = false;

					var resolveContext = solution != null ? document.GetContext(solution) : UniversalModuleReferenceContext.Instance;
					using (CompilationContextCookie.GetOrCreate(resolveContext)) {

						if (solution != null) {

							DocumentRange documentRange = textRange.CreateDocumentRange(document);
							IdentifierContentGroup contentGroup = GetIdentifierContentGroup(documentRange, solution, settings);
							if (contentGroup != null) {
								foreach (IdentifierTooltipContent content in contentGroup.Identifiers) {
									presenter.AddIdentifierTooltipContent(content);
									finalSpan = content.TrackingRange.ToSpan().Union(finalSpan);
									hasIdentifierTooltipContent = true;
								}
								if (contentGroup.ArgumentRole != null) {
									presenter.AddArgumentRoleTooltipContent(contentGroup.ArgumentRole);
									if (finalSpan == null) {
										// Only track the argument role if we have nothing else displayed.
										// See https://github.com/MrJul/ReSharper.EnhancedTooltip/issues/70
										finalSpan = contentGroup.ArgumentRole.TrackingRange.ToSpan();
									}
								}
							}
						}

						List<Vs10Highlighter> highlighters = documentMarkup.GetHighlightersOver(textRange).OfType<Vs10Highlighter>().ToList();
						foreach (Vs10Highlighter highlighter in highlighters) {
							IEnumerable<IReSharperTooltipContent> contents = GetTooltipContents(highlighter, highlighter.Range, documentMarkup, solution, hasIdentifierTooltipContent);
							foreach (IReSharperTooltipContent content in contents) {
								if (presenter.TryAddReSharperContent(content))
									finalSpan = content.TrackingRange.ToSpan().Union(finalSpan);
							}
						}

					}

					var vsSquiggleContents = session.RetrieveVsSquiggleContents()
						.OfType<string>()
						.ToSet();

					bool ignoredFirstVsElement = false;
					foreach (object content in quickInfoContent) {
						if (content == null)
							continue;

						// ignore existing R# elements
						if (content is IQuickInfoContent)
							continue;

						var contentFullName = content.GetType().FullName;

						// ignore the first VS element, as it's the identifier tooltip and we already have one
						if (hasIdentifierTooltipContent && !ignoredFirstVsElement) {
							
							if (contentFullName == VsFullTypeNames.ContainerElement /* VS 2017 >= 15.8 */
							|| contentFullName == VsFullTypeNames.QuickInfoDisplayPanel /* VS 2015 and VS 2017 < 15.8 */
							|| content is ITextBuffer /* VS2012 and VS2013 */) {
								ignoredFirstVsElement = true;
								continue;
							}

						}

						// ignore Roslyn's bulb info placeholder (interactive tooltip "press ctrl+.")
						if (contentFullName == VsFullTypeNames.LightBulbQuickInfoPlaceHolder)
							continue;

						if (contentFullName == VsFullTypeNames.QuickInfoDisplayPanel)
							presenter.AddVsIdentifierContent(new VsIdentifierContent(content));
						else if (content is string stringContent && vsSquiggleContents.Contains(stringContent))
							presenter.AddVsSquiggleContent(new VsSquiggleContent(stringContent));
						else
							presenter.AddVsUnknownContent(content);
					}

					quickInfoContent.Clear();
					quickInfoContent.AddRange(presenter.PresentContents());
				}
			}

			if (shellLocks.ReentrancyGuard.TryExecute("GetEnhancedTooltips", GetEnhancedTooltips) && finalSpan != null)
				applicableToSpan = textSnapshot.CreateTrackingSpan(finalSpan.Value, SpanTrackingMode.EdgeInclusive);
		}

		[CanBeNull]
		[Pure]
		private static IdentifierContentGroup GetIdentifierContentGroup(
			DocumentRange documentRange,
			[NotNull] ISolution solution,
			[NotNull] IContextBoundSettingsStore settings)
			=> solution
				.GetComponent<IdentifierTooltipContentProvider>()
				.GetIdentifierContentGroup(documentRange, settings);

		[NotNull]
		[ItemNotNull]
		[Pure]
		private static IEnumerable<IReSharperTooltipContent> GetTooltipContents(
			[NotNull] IHighlighter highlighter,
			TextRange range,
			[NotNull] IDocumentMarkup documentMarkup,
			[CanBeNull] ISolution solution,
			bool skipIdentifierHighlighting) {

			if (highlighter.Attributes.EffectType == EffectType.GUTTER_MARK)
				yield break;

			if (highlighter.UserData is IHighlighting highlighting) {
				
				IDocument document = documentMarkup.Document;
				IContextBoundSettingsStore settings = document.GetSettings();
				IPsiSourceFile sourceFile = solution != null ? document.GetPsiSourceFile(solution) : null;

				Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, highlighting.GetType(), sourceFile, settings);
				IssueTooltipContent issueContent = TryCreateIssueContent(highlighting, range, highlighter.TryGetTooltip(HighlighterTooltipKind.TextEditor), severity, settings, solution);
				if (issueContent != null) {
					yield return issueContent;
					yield break;
				}

				if (solution != null && IsIdentifierHighlighting(highlighting)) {
					if (!skipIdentifierHighlighting) {
						var identifierContentGroup = GetIdentifierContentGroup(highlighter, solution, settings);
						if (identifierContentGroup != null) {
							foreach (IdentifierTooltipContent content in identifierContentGroup.Identifiers)
								yield return content;
							if (identifierContentGroup.ArgumentRole != null)
								yield return identifierContentGroup.ArgumentRole;
						}
					}
					yield break;
				}
			}

			MiscTooltipContent miscContent = TryCreateMiscContent(highlighter.TryGetTooltip(HighlighterTooltipKind.TextEditor), range);
			if (miscContent != null)
				yield return miscContent;
		}

		[CanBeNull]
		private static ISolution TryGetCurrentSolution()
			=> Shell.Instance.TryGetComponent<VSSolutionManager>()?.CurrentSolution;

		[CanBeNull]
		[Pure]
		private static IdentifierContentGroup GetIdentifierContentGroup(
			[NotNull] IHighlighter highlighter,
			[NotNull] ISolution solution,
			[NotNull] IContextBoundSettingsStore settings)
			=> solution
				.GetComponent<IdentifierTooltipContentProvider>()
				.GetIdentifierContentGroup(highlighter, settings);

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
			SnapshotPoint currentPoint = session.GetTriggerPoint(TextBuffer).GetPoint(textSnapshot);
			var currentSpan = new SnapshotSpan(currentPoint, 0);
			return new TextRange(currentSpan.Start, currentSpan.End);
		}

		[Pure]
		private static bool IsIdentifierHighlighting([NotNull] IHighlighting highlighting) {
			var attribute = HighlightingSettingsManager.Instance.GetHighlightingAttribute(highlighting) as StaticSeverityHighlightingAttribute;
			return attribute?.GroupId == RegisterStaticHighlightingsGroupAttribute.GetGroupIdString(typeof(HighlightingGroupIds.IdentifierHighlightings));
		}

		public MainQuickInfoSource([NotNull] ITextBuffer textBuffer)
			: base(textBuffer) {
		}

	}

}