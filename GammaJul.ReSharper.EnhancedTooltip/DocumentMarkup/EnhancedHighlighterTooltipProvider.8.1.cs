using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Files;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	partial class EnhancedHighlighterTooltipProvider {

		private static bool AreAllDocumentsCommitted([NotNull] IPsiFiles psiFiles) {
			return psiFiles.AllDocumentsAreCommitted;
		}

	}
}
