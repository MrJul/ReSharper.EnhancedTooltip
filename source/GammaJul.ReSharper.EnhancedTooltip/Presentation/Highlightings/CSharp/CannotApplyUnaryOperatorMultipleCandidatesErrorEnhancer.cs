using System;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotApplyUnaryOperatorMultipleCandidatesErrorEnhancer : CSharpHighlightingEnhancer<CannotApplyUnaryOperatorMultipleCandidatesError> {

		protected override void AppendTooltip(CannotApplyUnaryOperatorMultipleCandidatesError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply operator '");
			colorizer.AppendOperator(highlighting.Sign);
			colorizer.AppendPlainText("' to operand of type '");
			colorizer.AppendExpressionType(highlighting.OperandType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("', ");
			colorizer.AppendPlainText(Environment.NewLine);
			colorizer.AppendPlainText("candidates are: ");
			colorizer.AppendRichText(highlighting.Candidates);
		}
		
		public CannotApplyUnaryOperatorMultipleCandidatesErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}