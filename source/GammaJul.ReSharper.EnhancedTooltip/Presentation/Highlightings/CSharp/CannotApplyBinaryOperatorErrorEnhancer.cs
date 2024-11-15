using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotApplyBinaryOperatorErrorEnhancer : CSharpHighlightingEnhancer<CannotApplyBinaryOperatorError> {

		protected override void AppendTooltip(CannotApplyBinaryOperatorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply operator '");
			colorizer.AppendOperator(highlighting.Sign.GetText());
			colorizer.AppendPlainText("' to operands of type '");
			colorizer.AppendExpressionType(highlighting.LeftType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendExpressionType(highlighting.RightType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotApplyBinaryOperatorErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}