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
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.Platform.VisualStudio.SinceVs10.Interop.Shim.TextControl;
using JetBrains.Platform.VisualStudio.SinceVs10.TextControl.Markup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VsIntegration.ProjectDocuments;
using JetBrains.VsIntegration.TextControl;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

  [ShellComponent]
  public class QuickInfoSessionsLifetimes {
    private SequentialLifetimes SequentialLifetimes { get; }
    public QuickInfoSessionsLifetimes(Lifetime lifetime) {
      SequentialLifetimes = new SequentialLifetimes(lifetime);
    }
    public Lifetime NextLifetime(ITextControl textControl) {
      return SequentialLifetimes.Next().Intersect(textControl.Lifetime);
    }
  }

  public class MyInterruptableReadActivity : InterruptableReadActivity 
  {

    public MyInterruptableReadActivity(MultipleTooltipContentPresenter presenter, IHighlighter highlighter, ShellLocks locks, Lifetime lifetime)
      : base(lifetime, locks) {

    }

    /// <inheritdoc />
    protected override void Start() {

    }

    protected override void Work() {
      //myRichTextBlock = myHighlihgter.TryGetTooltip(HighlighterTooltipKind.TextEditor);
      //myHighlihgter = null;
    }

    protected override void Finish()
    {
      //myPresenter.SetContent(myRichTextBlock);
    }

    /// <inheritdoc />
    protected override String ThreadName => "EnhancedToolTip";

  }

	public class MainQuickInfoSource : QuickInfoSourceBase {

		protected override void AugmentQuickInfoSessionCore(
			IQuickInfoSession session,
			IList<object?> quickInfoContent,
			IDocumentMarkup documentMarkup,
			TooltipFormattingProvider tooltipFormattingProvider,
			out ITrackingSpan? applicableToSpan) {

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
					ISolution? solution = MainQuickInfoSource.TryGetCurrentSolution();
					bool hasIdentifierTooltipContent = false;

					var resolveContext = solution is null ? UniversalModuleReferenceContext.Instance : document.GetContext(solution);
					using (CompilationContextCookie.GetOrCreate(resolveContext)) {

						if (solution is not null && MainQuickInfoSource.GetIdentifierContentGroup(textRange.CreateDocumentRange(document), solution, settings) is { } contentGroup) {
							foreach (IdentifierTooltipContent content in contentGroup.Identifiers) {
								presenter.AddIdentifierTooltipContent(content);
								finalSpan = content.TrackingRange.ToSpan().Union(finalSpan);
								hasIdentifierTooltipContent = true;
							}
							
							if (contentGroup.ArgumentRole is not null) {
								presenter.AddArgumentRoleTooltipContent(contentGroup.ArgumentRole);
								// Only track the argument role if we have nothing else displayed.
								// See https://github.com/MrJul/ReSharper.EnhancedTooltip/issues/70
								finalSpan ??= contentGroup.ArgumentRole.TrackingRange.ToSpan();
							}
						}

						var highlighters = documentMarkup.GetHighlightersOver(textRange).ToList();
						foreach (var highlighter in highlighters) {
							IEnumerable<IReSharperTooltipContent> contents = MainQuickInfoSource.GetTooltipContents(highlighter, highlighter.Range, documentMarkup, solution, hasIdentifierTooltipContent);
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
					foreach (object? content in quickInfoContent) {
						if (content is null)
							continue;

						// ignore existing R# elements
						if (content is IQuickInfoContent)
							continue;

						var contentFullName = content.GetType().FullName;
						if (content.ToString().Contains("quickinfo:")) {
							continue;
						}

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

			if (shellLocks.ReentrancyGuard.TryExecute("GetEnhancedTooltips", GetEnhancedTooltips) && finalSpan is not null)
				applicableToSpan = textSnapshot.CreateTrackingSpan(finalSpan.Value, SpanTrackingMode.EdgeInclusive);
		}

		[Pure]
		private static IdentifierContentGroup? GetIdentifierContentGroup(
			DocumentRange documentRange,
			ISolution solution,
			IContextBoundSettingsStore settings)
			=> solution
				.GetComponent<IdentifierTooltipContentProvider>()
				.GetIdentifierContentGroup(documentRange, settings);

		[Pure]
		private static IEnumerable<IReSharperTooltipContent> GetTooltipContents(
			IHighlighter highlighter,
			TextRange range,
			IDocumentMarkup documentMarkup,
			ISolution? solution,
			bool skipIdentifierHighlighting) {

			if (highlighter.Attributes.EffectType == EffectType.GUTTER_MARK)
				yield break;

			if (highlighter.UserData is IHighlighting highlighting) {
				
				IDocument document = documentMarkup.Document;
				IContextBoundSettingsStore settings = document.GetSettings();
				IPsiSourceFile? sourceFile = solution is null ? null : document.GetPsiSourceFile(solution);

				Severity severity = HighlightingSettingsManager.Instance.GetSeverity(highlighting, highlighting.GetType(), sourceFile, settings);
				if (MainQuickInfoSource.TryCreateIssueContent(highlighting, range, highlighter.TryGetTooltip(HighlighterTooltipKind.TextEditor), severity, settings, solution) is { } issueContent) {
					yield return issueContent;
					yield break;
				}

				if (solution is not null && MainQuickInfoSource.IsIdentifierHighlighting(highlighting)) {
					if (!skipIdentifierHighlighting) {
						var identifierContentGroup = MainQuickInfoSource.GetIdentifierContentGroup(highlighter, solution, settings);
						if (identifierContentGroup is not null) {
							foreach (IdentifierTooltipContent content in identifierContentGroup.Identifiers)
								yield return content;
							if (identifierContentGroup.ArgumentRole is not null)
								yield return identifierContentGroup.ArgumentRole;
						}
					}
					yield break;
				}
			}

			if (MainQuickInfoSource.TryCreateMiscContent(highlighter.TryGetTooltip(HighlighterTooltipKind.TextEditor), range) is { } miscContent)
				yield return miscContent;
		}

		private static ISolution? TryGetCurrentSolution()
			=> Shell.Instance.TryGetComponent<VSSolutionManager>()?.CurrentSolution;

		[Pure]
		private static IdentifierContentGroup? GetIdentifierContentGroup(
			IHighlighter highlighter,
			ISolution solution,
			IContextBoundSettingsStore settings)
			=> solution
				.GetComponent<IdentifierTooltipContentProvider>()
				.GetIdentifierContentGroup(highlighter, settings);

		[Pure]
		private static IssueTooltipContent? TryCreateIssueContent(
			IHighlighting highlighting,
			TextRange trackingRange,
			RichTextBlock? textBlock,
			Severity severity,
			IContextBoundSettingsStore settings,
			ISolution? solution) {

			if (textBlock is null || !severity.IsIssue())
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			if (settings.GetValue((IssueTooltipSettings s) => s.ColorizeElementsInErrors)) {
				RichText? enhancedText = MainQuickInfoSource.TryEnhanceHighlighting(highlighting, settings, solution);
				if (!enhancedText.IsNullOrEmpty())
					text = enhancedText!;
			}

			var issueContent = new IssueTooltipContent(text, trackingRange);
			if (settings.GetValue((IssueTooltipSettings s) => s.ShowIcon))
				issueContent.Icon = severity.TryGetIcon();
			return issueContent;
		}

		private static RichText? TryEnhanceHighlighting(
			IHighlighting highlighting,
			IContextBoundSettingsStore settings,
			ISolution? solution)
			=> solution
				?.TryGetComponent<HighlightingEnhancerManager>()
				?.TryEnhance(highlighting, settings);

		[Pure]
		private static MiscTooltipContent? TryCreateMiscContent(RichTextBlock? textBlock, TextRange range) {
			if (textBlock is null)
				return null;

			RichText text = textBlock.RichText;
			if (text.IsEmpty)
				return null;

			return new MiscTooltipContent(text, range);
		}

		[Pure]
		private TextRange GetCurrentTextRange(IIntellisenseSession session, ITextSnapshot textSnapshot) {
			SnapshotPoint currentPoint = session.GetTriggerPoint(TextBuffer).GetPoint(textSnapshot);
			var currentSpan = new SnapshotSpan(currentPoint, 0);
			return new TextRange(currentSpan.Start, currentSpan.End);
		}

		[Pure]
		private static bool IsIdentifierHighlighting(IHighlighting highlighting) {
			var attribute = HighlightingSettingsManager.Instance.GetHighlightingAttribute(highlighting) as StaticSeverityHighlightingAttribute;
			return attribute?.GroupId == RegisterStaticHighlightingsGroupAttribute.GetGroupIdString(typeof(HighlightingGroupIds.IdentifierHighlightings));
		}

		public MainQuickInfoSource(ITextBuffer textBuffer)
			: base(textBuffer) {
		}

	}

}