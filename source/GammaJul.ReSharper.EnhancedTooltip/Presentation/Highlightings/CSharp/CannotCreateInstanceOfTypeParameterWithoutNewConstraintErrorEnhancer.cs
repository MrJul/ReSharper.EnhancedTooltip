using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotCreateInstanceOfTypeParameterWithoutNewConstraintErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfTypeParameterWithoutNewConstraintError> {

		protected override void AppendTooltip(CannotCreateInstanceOfTypeParameterWithoutNewConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot create an instance of the type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Expression);
			colorizer.AppendPlainText("' because it does not have the ");
			colorizer.AppendKeyword("new");
			colorizer.AppendPlainText("() constraint");
		}
		
		public CannotCreateInstanceOfTypeParameterWithoutNewConstraintErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}