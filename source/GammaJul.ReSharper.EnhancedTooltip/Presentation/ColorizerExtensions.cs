using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static class ColorizerExtensions {

		[NotNull]
		public static PresentedInfo TryAppendDeclaredElement(
			[NotNull] this IColorizer colorizer,
			[CanBeNull] IDeclaredElement element,
			[CanBeNull] ISubstitution substitution,
			[NotNull] PresenterOptions options,
			[CanBeNull] ITreeNode contextualNode) {

			if (element == null || substitution == null)
				return new PresentedInfo();

			return colorizer.AppendDeclaredElement(element, substitution, options, contextualNode);
		}

	}

}