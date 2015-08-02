using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Resolve;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	[Language(typeof(CSharpLanguage))]
	internal sealed class CSharpInvocatiionCandidateCountProvider : IInvocationCandidateCountProvider {

		public int? TryGetInvocationCandidateCount(IReference reference) {
			return FindInvocationReference(reference)?.GetCandidates().Count();
		}
		
		[CanBeNull]
		private static ICSharpInvocationReference FindInvocationReference([NotNull] IReference reference) {
			ICSharpInvocationReference invocationReference = reference as ICSharpInvocationReference;
			if (invocationReference != null)
				return invocationReference;

			ITreeNode treeNode = reference.GetTreeNode();
			var argumentsOwner = treeNode.GetContainingNode<ICSharpArgumentsOwner>();
			ITokenNode lBound = argumentsOwner?.LBound;
			if (lBound != null && treeNode.GetTreeEndOffset() <= lBound.GetTreeStartOffset())
				return argumentsOwner.Reference;

			return null;
		}

	}

}