using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static class ColorizerExtensions {

		public static PresentedInfo TryAppendDeclaredElement(
			this IColorizer colorizer,
			IDeclaredElement? element,
			ISubstitution? substitution,
			PresenterOptions options,
			ITreeNode? contextualNode) {

			if (element is null || substitution is null)
				return new PresentedInfo();

			return colorizer.AppendDeclaredElement(element, substitution, options, contextualNode);
		}

	}

}