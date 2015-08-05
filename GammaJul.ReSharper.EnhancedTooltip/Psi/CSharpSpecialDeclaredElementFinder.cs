using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	[Language(typeof(CSharpLanguage))]
	internal sealed class CSharpSpecialDeclaredElementFinder : ISpecialDeclaredElementFinder {

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

			var creation = newKeyword.Parent as IObjectCreationExpression;
			if (creation == null)
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

	}

}