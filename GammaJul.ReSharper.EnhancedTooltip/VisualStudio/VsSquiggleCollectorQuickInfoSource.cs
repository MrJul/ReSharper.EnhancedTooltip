using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.TextControl.DocumentMarkup;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	/// <summary>
	/// This class only store squiggles added by Roslyn inside the session, so they can be identifier easily later.
	/// </summary>
	public class VsSquiggleCollectorQuickInfoSource : QuickInfoSourceBase {

		protected override void AugmentQuickInfoSessionCore(
			IQuickInfoSession session,
			IList<object> quickInfoContent,
			IDocumentMarkup documentMarkup,
			TooltipFormattingProvider tooltipFontProvider,
			out ITrackingSpan applicableToSpan) {

			applicableToSpan = null;

			session.StoreVsSquiggleContents(new HashSet<object>(quickInfoContent));
		}
		
		public VsSquiggleCollectorQuickInfoSource([NotNull] IVsEditorAdaptersFactoryService editorAdaptersFactoryService, [NotNull] ITextBuffer textBuffer)
			: base(editorAdaptersFactoryService, textBuffer) {
		}

	}

}