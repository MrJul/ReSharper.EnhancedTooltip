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

		public DeclaredElementInstance? FindDeclaredElement(ITreeNode node, IFile file, out TextRange sourceRange) {
			TokenNodeType? tokenType = node.GetTokenType();

			if (tokenType == CSharpTokenType.VAR_KEYWORD)
				return FindElementFromVarKeyword(node, file, out sourceRange);

			if (tokenType == CSharpTokenType.NEW_KEYWORD)
				return FindElementFromNewKeyword(node, file, out sourceRange);
			
			sourceRange = TextRange.InvalidRange;
			return null;
		}

		private static DeclaredElementInstance? FindElementFromVarKeyword(ITreeNode varKeyword, IFile file, out TextRange sourceRange) {
			sourceRange = TextRange.InvalidRange;

			IDeclaredElement? declaredElement = (varKeyword.Parent as IMultipleLocalVariableDeclaration)
				?.DeclaratorsEnumerable
				.FirstOrDefault()
				?.DeclaredElement;

			if (declaredElement is null)
				return null;

			sourceRange = file.GetDocumentRange(varKeyword.GetTreeTextRange()).TextRange;
			return new DeclaredElementInstance(declaredElement, EmptySubstitution.INSTANCE);
		}

		private static DeclaredElementInstance? FindElementFromNewKeyword(ITreeNode newKeyword, IFile file, out TextRange sourceRange) {
			if (newKeyword.Parent is IObjectCreationExpression creation
			&& (TryResolveReference(creation.ConstructorReference) ?? TryResolveReference(creation.TypeReference)) is { } instance) {
				sourceRange = file.GetDocumentRange(newKeyword.GetTreeTextRange()).TextRange;
				return instance;
			}

			sourceRange = TextRange.InvalidRange;
			return null;
		}

		private static DeclaredElementInstance? TryResolveReference(IReference? reference)
			=> reference?.Resolve().Result is { DeclaredElement: not null } resolveResult
				? new DeclaredElementInstance(resolveResult.DeclaredElement, resolveResult.Substitution)
				: null;

		public PresentableNode FindPresentableNode(ITreeNode node) {
			if (FindTupleTypeComponent(node) is { } tupleComponent)
				return new PresentableNode(tupleComponent, PsiSymbolsThemedIcons.Field.Id);

			if (FindLiteralExpression(node) is { } literalExpression)
				return new PresentableNode(literalExpression, PsiSymbolsThemedIcons.LocalConst.Id);

			return default;
		}

		private static ITreeNode? FindTupleTypeComponent(ITreeNode node)
			=> node.GetContainingNode<ITupleTypeComponent>(true);

		private static ITreeNode? FindLiteralExpression(ITreeNode node) {
			if (node.GetContainingNode<ILiteralExpression>(true) is not { } literalExpression)
				return null;

			TreeTextRange literalRange = literalExpression.Literal.GetTreeTextRange();
			if (!literalRange.IntersectsOrContacts(node.GetTreeTextRange()))
				return null;

			return literalExpression;
		}

	}

}