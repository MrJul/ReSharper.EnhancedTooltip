using JetBrains.ReSharper.Psi.Tree;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public readonly struct PresentableNode {

		public readonly ITreeNode? Node;
		
		public readonly IconId? Icon;
		
		public PresentableNode(ITreeNode? node, IconId? icon) {
			Node = node;
			Icon = icon;
		}

	}

}