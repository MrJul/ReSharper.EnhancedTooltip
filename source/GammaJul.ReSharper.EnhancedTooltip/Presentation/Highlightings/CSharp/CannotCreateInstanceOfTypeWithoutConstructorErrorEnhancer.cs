using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotCreateInstanceOfTypeWithoutConstructorErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfTypeWithoutConstructorError> {

		protected override void AppendTooltip(CannotCreateInstanceOfTypeWithoutConstructorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The ");
			colorizer.AppendElementKind(highlighting.TypeElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.TypeElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.ObjectCreationExpression);
			colorizer.AppendPlainText("' has no constructors defined");
		}
		
		public CannotCreateInstanceOfTypeWithoutConstructorErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}