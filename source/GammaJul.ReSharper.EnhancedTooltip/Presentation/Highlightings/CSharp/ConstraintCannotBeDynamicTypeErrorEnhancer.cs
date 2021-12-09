using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConstraintCannotBeDynamicTypeErrorEnhancer : CSharpHighlightingEnhancer<ConstraintCannotBeDynamicTypeError> {

		protected override void AppendTooltip(ConstraintCannotBeDynamicTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Constraint cannot be a ");
			colorizer.AppendKeyword("dynamic");
			colorizer.AppendPlainText(" type '");
			colorizer.AppendExpressionType(highlighting.SuperType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public ConstraintCannotBeDynamicTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}