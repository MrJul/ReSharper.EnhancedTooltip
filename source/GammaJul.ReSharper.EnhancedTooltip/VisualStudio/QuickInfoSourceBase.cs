using JetBrains.Annotations;
using JetBrains.DocumentModel;
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

		[NotNull]
		protected ITextBuffer TextBuffer { get; }

		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
			applicableToSpan = null;

			if (session == null || session.IsDismissed || session.TextView.TextBuffer != TextBuffer || quickInfoContent == null || quickInfoContent.IsReadOnly)
				return;

			IDocumentMarkup documentMarkup = TryGetDocumentMarkup(session.TextView);
			if (documentMarkup == null)
				return;

			// If this fails, it means the extension is disabled and none of the components are available.
			var tooltipFontProvider = Shell.Instance.TryGetComponent<TooltipFormattingProvider>();
			if (tooltipFontProvider == null)
				return;

			AugmentQuickInfoSessionCore(session, quickInfoContent, documentMarkup, tooltipFontProvider, out applicableToSpan);
		}

		[CanBeNull]
		[Pure]
		private IDocumentMarkup TryGetDocumentMarkup([CanBeNull] ITextView textView) {
			IDocument document = VsTextViewSolutionContextProvider.TryGetContext(textView)?.TextControl.Document;
			if (document == null)
				return null;

			IDocumentMarkup documentMarkup = Shell.Instance.TryGetComponent<IDocumentMarkupManager>()?.TryGetMarkupModel(document);
			return documentMarkup is VsDocumentMarkupDevTen ? documentMarkup : null;
		}

		protected abstract void AugmentQuickInfoSessionCore(
			[NotNull] IQuickInfoSession session,
			[NotNull] IList<object> quickInfoContent,
			[NotNull] IDocumentMarkup documentMarkup,
			[NotNull] TooltipFormattingProvider tooltipFormattingProvider,
			out ITrackingSpan applicableToSpan);

		public void Dispose() {
		}

		protected QuickInfoSourceBase([NotNull] ITextBuffer textBuffer) {
			TextBuffer = textBuffer;
		}

	}

}