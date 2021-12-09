using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfStatementToMethodCallWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToMethodCallWarning> {

		protected override void AppendTooltip(ConvertIfStatementToMethodCallWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to method call with '");
			colorizer.AppendOperator("?:");
			colorizer.AppendPlainText("' expression inside");
		}
		
		public ConvertIfStatementToMethodCallWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}