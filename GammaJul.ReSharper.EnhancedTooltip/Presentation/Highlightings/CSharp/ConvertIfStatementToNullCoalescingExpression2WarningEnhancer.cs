using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfStatementToNullCoalescingExpression2WarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToNullCoalescingExpression2Warning> {

		protected override void AppendTooltip(ConvertIfStatementToNullCoalescingExpression2Warning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to '");
			colorizer.AppendOperator("??");
			colorizer.AppendPlainText("' expression");
		}
		
		public ConvertIfStatementToNullCoalescingExpression2WarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}