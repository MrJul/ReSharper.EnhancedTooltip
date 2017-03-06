using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfToOrExpressionWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfToOrExpressionWarning> {

		protected override void AppendTooltip(ConvertIfToOrExpressionWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to '");
			colorizer.AppendOperator("||");
			colorizer.AppendPlainText("' expression");
		}
		
		public ConvertIfToOrExpressionWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}