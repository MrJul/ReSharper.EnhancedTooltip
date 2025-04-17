using System;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotApplyBinaryOperatorMultipleCandidatesErrorEnhancer : CSharpHighlightingEnhancer<CannotApplyBinaryOperatorMultipleCandidatesError> {

		protected override void AppendTooltip(CannotApplyBinaryOperatorMultipleCandidatesError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply operator '");
			colorizer.AppendOperator(highlighting.Sign.GetText());
			colorizer.AppendPlainText("' to operands of type '");
			colorizer.AppendExpressionType(highlighting.LeftType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendExpressionType(highlighting.RightType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("', ");
			colorizer.AppendPlainText(Environment.NewLine);
			colorizer.AppendPlainText("candidates are: ");
			colorizer.AppendRichTextToText(highlighting.Candidates);
		}
		
		public CannotApplyBinaryOperatorMultipleCandidatesErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}