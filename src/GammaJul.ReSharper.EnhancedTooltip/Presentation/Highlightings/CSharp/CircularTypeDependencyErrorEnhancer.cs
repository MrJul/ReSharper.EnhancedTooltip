using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {
	
	[SolutionComponent]
	internal sealed class CircularTypeDependencyErrorEnhancer : CSharpHighlightingEnhancer<CircularTypeDependencyError> {

		protected override void AppendTooltip(CircularTypeDependencyError highlighting, CSharpColorizer colorizer) {
			IClassLikeDeclaration declaration = highlighting.Declaration;
			ITypeElement declaredElement = declaration.DeclaredElement;
			if (declaredElement == null)
				return;

			colorizer.AppendPlainText("Circular ");
			colorizer.AppendElementKind(declaredElement);
			colorizer.AppendPlainText(" dependency involving '");
			colorizer.AppendDeclaredElement(declaredElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, declaration);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendDeclaredElement(highlighting.SuperClass, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, declaration);
			colorizer.AppendPlainText("'");
		}
		
		public CircularTypeDependencyErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}