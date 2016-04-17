using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotApplyUnaryOperatorErrorEnhancer : CSharpHighlightingEnhancer<CannotApplyUnaryOperatorError> {

		protected override void AppendTooltip(CannotApplyUnaryOperatorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply operator '");
			colorizer.AppendOperator(highlighting.Sign);
			colorizer.AppendPlainText("' to operand of type '");
			colorizer.AppendExpressionType(highlighting.OperandType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotApplyUnaryOperatorErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}