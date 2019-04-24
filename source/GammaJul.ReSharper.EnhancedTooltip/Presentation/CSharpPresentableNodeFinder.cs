using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[Language(typeof(CSharpLanguage))]
	internal sealed class CSharpPresentableNodeFinder : IPresentableNodeFinder {

		public DeclaredElementInstance FindDeclaredElement(ITreeNode node, IFile file, out TextRange sourceRange) {
			TokenNodeType tokenType = node.GetTokenType();

			if (tokenType == CSharpTokenType.VAR_KEYWORD)
				return FindElementFromVarKeyword(node, file, out sourceRange);

			if (tokenType == CSharpTokenType.NEW_KEYWORD)
				return FindElementFromNewKeyword(node, file, out sourceRange);
			
			sourceRange = TextRange.InvalidRange;
			return null;
		}

		[CanBeNull]
		private static DeclaredElementInstance FindElementFromVarKeyword([NotNull] ITreeNode varKeyword, [NotNull] IFile file, out TextRange sourceRange) {
			sourceRange = TextRange.InvalidRange;

			IDeclaredElement declaredElement = (varKeyword.Parent as IMultipleLocalVariableDeclaration)
				?.DeclaratorsEnumerable
				.FirstOrDefault()
				?.DeclaredElement;

			if (declaredElement == null)
				return null;

			sourceRange = file.GetDocumentRange(varKeyword.GetTreeTextRange()).TextRange;
			return new DeclaredElementInstance(declaredElement, EmptySubstitution.INSTANCE);
		}

		[CanBeNull]
		private static DeclaredElementInstance FindElementFromNewKeyword([NotNull] ITreeNode newKeyword, [NotNull] IFile file, out TextRange sourceRange) {
			sourceRange = TextRange.InvalidRange;

			if (!(newKeyword.Parent is IObjectCreationExpression creation))
				return null;

			DeclaredElementInstance instance = TryResolveReference(creation.ConstructorReference) ?? TryResolveReference(creation.TypeReference);
			if (instance == null)
				return null;

			sourceRange = file.GetDocumentRange(newKeyword.GetTreeTextRange()).TextRange;
			return instance;
		}

		[CanBeNull]
		private static DeclaredElementInstance TryResolveReference([CanBeNull] IReference reference) {
			IResolveResult resolveResult = reference?.Resolve().Result;
			if (resolveResult?.DeclaredElement != null)
				return new DeclaredElementInstance(resolveResult.DeclaredElement, resolveResult.Substitution);
			return null;
		}

		public PresentableNode FindPresentableNode(ITreeNode node) {
			var tupleComponent = FindTupleTypeComponent(node);
			if (tupleComponent != null)
				return new PresentableNode(tupleComponent, PsiSymbolsThemedIcons.Field.Id);

			var literalExpression = FindLiteralExpression(node);
			if (literalExpression != null)
				return new PresentableNode(literalExpression, PsiSymbolsThemedIcons.LocalConst.Id);

			return default;
		}

		[CanBeNull]
		private static ITreeNode FindTupleTypeComponent([NotNull] ITreeNode node)
			=> node.GetContainingNode<ITupleTypeComponent>(true);

		[CanBeNull]
		private static ITreeNode FindLiteralExpression([NotNull] ITreeNode node) {
			var literalExpression = node.GetContainingNode<ILiteralExpression>(true);
			if (literalExpression == null)
				return null;

			TreeTextRange literalRange = literalExpression.Literal.GetTreeTextRange();
			if (!literalRange.IntersectsOrContacts(node.GetTreeTextRange()))
				return null;

			return literalExpression;
		}

	}

}