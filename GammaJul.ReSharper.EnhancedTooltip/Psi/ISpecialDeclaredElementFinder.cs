using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	/// <summary>
	/// Implement to find an <see cref="IDeclaredElement"/> that isn't coming from a reference nor a declaration.
	/// </summary>
	internal interface ISpecialDeclaredElementFinder {

		[CanBeNull]
		DeclaredElementInstance FindDeclaredElement([NotNull] ITreeNode node, [NotNull] IFile file, out TextRange sourceRange);

	}

}