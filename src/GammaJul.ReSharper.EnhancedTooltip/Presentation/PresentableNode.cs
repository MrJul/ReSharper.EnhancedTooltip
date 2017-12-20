using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public struct PresentableNode {

		[CanBeNull] public readonly ITreeNode Node;
		
		[CanBeNull] public readonly IconId Icon;
		
		public PresentableNode([CanBeNull] ITreeNode node, [CanBeNull] IconId icon) {
			Node = node;
			Icon = icon;
		}


	}

}