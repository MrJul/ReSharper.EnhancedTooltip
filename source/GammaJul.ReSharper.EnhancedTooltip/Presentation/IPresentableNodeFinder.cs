using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// Implement to find an <see cref="IDeclaredElement"/> or presentable <see cref="ITreeNode"/> that isn't coming from a reference nor a declaration.
	/// </summary>
	internal interface IPresentableNodeFinder {

		DeclaredElementInstance? FindDeclaredElement(ITreeNode node, IFile file, out TextRange sourceRange);

		PresentableNode FindPresentableNode(ITreeNode node);

	}

}