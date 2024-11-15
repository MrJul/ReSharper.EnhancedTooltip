using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConvertIfStatementToMethodCallWithAssignmentWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToMethodCallWithAssignmentWarning> {

		protected override void AppendTooltip(ConvertIfStatementToMethodCallWithAssignmentWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to method call with '");
			colorizer.AppendOperator("?:");
			colorizer.AppendPlainText("' expression inside");
		}
		
		public ConvertIfStatementToMethodCallWithAssignmentWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}