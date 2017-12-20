using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using JetVsTextBuffer = JetBrains.VsIntegration.Interop.Shim.TextManager.Documents.IVsTextBuffer;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public abstract class QuickInfoSourceBase : IQuickInfoSource {

		[NotNull] private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
		[NotNull] private readonly ITextBuffer _textBuffer;

		[NotNull]
		protected ITextBuffer TextBuffer
			=> _textBuffer;

		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan) {
			applicableToSpan = null;

			if (session == null || quickInfoContent == null || quickInfoContent.IsReadOnly)
				return;

			IDocumentMarkup documentMarkup = TryGetDocumentMarkup();
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
		private IDocumentMarkup TryGetDocumentMarkup() {
			IVsTextBuffer bufferAdapter = _editorAdaptersFactoryService.GetBufferAdapter(_textBuffer);
			if (bufferAdapter == null)
				return null;

			IDocument document = new JetVsTextBuffer(bufferAdapter).JetDocument.Value;
			if (document == null)
				return null;

			IDocumentMarkup documentMarkup = Shell.Instance.TryGetComponent<IDocumentMarkupManager>()?.TryGetMarkupModel(document);
			if (documentMarkup == null || !documentMarkup.GetType().Name.StartsWith("Vs", StringComparison.Ordinal))
				return null;
			
			return documentMarkup;
		}

		protected abstract void AugmentQuickInfoSessionCore(
			[NotNull] IQuickInfoSession session,
			[NotNull] IList<object> quickInfoContent,
			[NotNull] IDocumentMarkup documentMarkup,
			[NotNull] TooltipFormattingProvider tooltipFormattingProvider,
			out ITrackingSpan applicableToSpan);

		protected QuickInfoSourceBase([NotNull] IVsEditorAdaptersFactoryService editorAdaptersFactoryService, [NotNull] ITextBuffer textBuffer) {
			_editorAdaptersFactoryService = editorAdaptersFactoryService;
			_textBuffer = textBuffer;
		}

		public void Dispose() {
		}

	}

}