using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConditionIsAlwaysTrueOrFalseWarningEnhancer : CSharpHighlightingEnhancer<ConditionIsAlwaysTrueOrFalseWarning> {

		protected override void AppendTooltip(ConditionIsAlwaysTrueOrFalseWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Expression is always ");
			if (highlighting.ExpressionConstantValue == ConstantExpressionValue.TRUE)
				colorizer.AppendKeyword("true");
			else if (highlighting.ExpressionConstantValue == ConstantExpressionValue.FALSE)
				colorizer.AppendKeyword("false");
			else
				colorizer.AppendPlainText("$unknown$");
		}
		
		public ConditionIsAlwaysTrueOrFalseWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}