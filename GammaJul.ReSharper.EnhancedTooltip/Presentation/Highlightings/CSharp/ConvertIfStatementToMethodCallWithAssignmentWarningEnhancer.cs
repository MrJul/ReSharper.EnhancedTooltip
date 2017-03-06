using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfStatementToMethodCallWithAssignmentWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToMethodCallWithAssignmentWarning> {

		protected override void AppendTooltip(ConvertIfStatementToMethodCallWithAssignmentWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to method call with '");
			colorizer.AppendOperator("?:");
			colorizer.AppendPlainText("' expression inside");
		}
		
		public ConvertIfStatementToMethodCallWithAssignmentWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}