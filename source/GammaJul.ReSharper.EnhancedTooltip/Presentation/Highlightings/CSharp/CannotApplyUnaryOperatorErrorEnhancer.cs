using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotApplyUnaryOperatorErrorEnhancer : CSharpHighlightingEnhancer<CannotApplyUnaryOperatorError> {

		protected override void AppendTooltip(CannotApplyUnaryOperatorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply operator '");
			colorizer.AppendOperator(highlighting.Sign);
			colorizer.AppendPlainText("' to operand of type '");
			colorizer.AppendExpressionType(highlighting.OperandType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotApplyUnaryOperatorErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}