using System;
using System.Collections.Generic;
using System.Linq;
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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Avalon.Controls;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VsIntegration.ProjectDocuments;
using Microsoft.VisualStudio.Editor;
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
            IShellLocks shellLocks = ((DocumentMarkupBase)documentMarkup).Context.Locks;
			Span? finalSpan = null;

			Action getEnhancedTooltips = () => {
				using (shellLocks.UsingReadLock()) {

					IDocument document = documentMarkup.Document;
					var presenter = new MultipleTooltipContentPresenter(tooltipFormattingProvider.GetTooltipFormatting(), document);
					IContextBoundSettingsStore settings = document.GetSettings();
					ISolution solution = TryGetCurrentSolution();

					bool hasIdentifierTooltipContent = false;
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

					var vsSquiggleContents = session.RetrieveVsSquiggleContents()
						.OfType<string>()
						.ToHashSet();
					
					bool ignoredFirstTextBuffer = false;
					foreach (object content in quickInfoContent) {
						if (content == null)
							continue;

						// ignore existing R# elements
						if (content is RichTextPresenter)
							continue;

						var contentFullName = content.GetType().FullName;

						if (hasIdentifierTooltipContent) {

							// ignore Roslyn identifier tooltip (for VS2015)
							if (contentFullName == VsFullTypeNames.QuickInfoDisplayPanel)
								continue;

							// ignore the first VS text buffer (for VS2012 and VS2013)
							if (content is ITextBuffer && !ignoredFirstTextBuffer) {
								ignoredFirstTextBuffer = true;
								continue;
							}

						}

						if (contentFullName == VsFullTypeNames.LightBulbQuickInfoPlaceHolder) {
							// ignore Roslyn's bulb info placeholder (interactive tooltip "press ctrl+.")
							continue;
							
						}

						if (contentFullName == VsFullTypeNames.QuickInfoDisplayPanel)
							presenter.AddVsIdentifierContent(new VsIdentifierContent(content));
						else if (vsSquiggleContents.Contains(content))
							presenter.AddVsSquiggleContent(new VsSquiggleContent(content));
						else
							presenter.AddVsUnknownContent(content);
					}

					quickInfoContent.Clear();
					quickInfoContent.AddRange(presenter.PresentContents());
				}
			};

			if (shellLocks.ReentrancyGuard.TryExecute("GetEnhancedTooltips", getEnhancedTooltips) && finalSpan != null)
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
		[Pure]
		private static IEnumerable<IReSharperTooltipContent> GetTooltipContents(
			[NotNull] IHighlighter highlighter,
			TextRange range,
			[NotNull] IDocumentMarkup documentMarkup,
			[CanBeNull] ISolution solution,
			bool skipIdentifierHighlighting) {

			if (highlighter.Attributes.Effect.Type == EffectType.GUTTER_MARK)
				yield break;

			var highlighting = highlighter.UserData as IHighlighting;
			if (highlighting != null) {
				
				IDocument document = documentMarkup.Document;
				IContextBoundSettingsStore settings = document.GetSettings();

				Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, solution);
				IssueTooltipContent issueContent = TryCreateIssueContent(highlighting, range, highlighter.RichTextToolTip, severity, settings, solution);
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

			MiscTooltipContent miscContent = TryCreateMiscContent(highlighter.RichTextToolTip, range);
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
			return attribute?.GroupId == HighlightingGroupIds.IdentifierHighlightingsGroup;
		}

		public MainQuickInfoSource([NotNull] IVsEditorAdaptersFactoryService editorAdaptersFactoryService, [NotNull] ITextBuffer textBuffer)
			: base(editorAdaptersFactoryService, textBuffer) {
		}

	}

}