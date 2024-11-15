using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {
	
	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CircularTypeDependencyErrorEnhancer : CSharpHighlightingEnhancer<CircularTypeDependencyError> {

		protected override void AppendTooltip(CircularTypeDependencyError highlighting, CSharpColorizer colorizer) {
			IClassLikeDeclaration declaration = highlighting.Declaration;
			if (declaration.DeclaredElement is not { } declaredElement)
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
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}