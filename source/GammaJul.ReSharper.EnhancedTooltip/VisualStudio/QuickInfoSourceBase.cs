using JetBrains.Annotations;
using JetBrains.Platform.VisualStudio.SinceVs10.TextControl.Markup;
using JetBrains.PsiFeatures.VisualStudio.SinceVs10.TextControl.Intellisense;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public abstract class QuickInfoSourceBase : IQuickInfoSource {

		protected ITextBuffer TextBuffer { get; }

		public void AugmentQuickInfoSession(IQuickInfoSession? session, IList<object?>? quickInfoContent, out ITrackingSpan? applicableToSpan) {
			if (session is not (null or { IsDismissed: true })
			&& session.TextView.TextBuffer == TextBuffer
			&& quickInfoContent is not (null or { IsReadOnly: true }) 
			&& TryGetDocumentMarkup(session.TextView) is { } documentMarkup
			// If this fails, it means the extension is disabled and none of the components are available.
			&& Shell.Instance.TryGetComponent<TooltipFormattingProvider>() is { } tooltipFormattingProvider) {
				AugmentQuickInfoSessionCore(session, quickInfoContent, documentMarkup, tooltipFormattingProvider, out applicableToSpan);
			}
			else {
				applicableToSpan = null;
			}
		}

		[Pure]
		private static IDocumentMarkup? TryGetDocumentMarkup(ITextView? textView)
			=> VsTextViewSolutionContextProvider.TryGetContext(textView)?.TextControl.Document is { } document
				&& Shell.Instance.TryGetComponent<IDocumentMarkupManager>()?.TryGetMarkupModel(document) is VsDocumentMarkupDevTen documentMarkup
					? documentMarkup
					: null;

		protected abstract void AugmentQuickInfoSessionCore(
			IQuickInfoSession session,
			IList<object?> quickInfoContent,
			IDocumentMarkup documentMarkup,
			TooltipFormattingProvider tooltipFormattingProvider,
			out ITrackingSpan? applicableToSpan);

		public void Dispose() {
		}

		protected QuickInfoSourceBase(ITextBuffer textBuffer)
			=> TextBuffer = textBuffer;

	}

}