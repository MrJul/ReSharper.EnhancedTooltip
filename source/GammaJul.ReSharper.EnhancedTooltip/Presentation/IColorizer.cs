using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal interface IColorizer {

		PresentedInfo AppendDeclaredElement(
			IDeclaredElement element,
			ISubstitution substitution,
			PresenterOptions options,
			ITreeNode? contextualNode);
		
		void AppendPresentableNode(
			ITreeNode treeNode,
			PresenterOptions options);

	}

}