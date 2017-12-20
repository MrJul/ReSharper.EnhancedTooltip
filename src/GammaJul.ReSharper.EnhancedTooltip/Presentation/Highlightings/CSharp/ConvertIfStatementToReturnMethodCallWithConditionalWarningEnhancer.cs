using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfStatementToReturnMethodCallWithConditionalWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToReturnMethodCallWithConditionalWarning> {

		protected override void AppendTooltip(ConvertIfStatementToReturnMethodCallWithConditionalWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to '");
			colorizer.AppendKeyword("return");
			colorizer.AppendPlainText("' statement");
		}
		
		public ConvertIfStatementToReturnMethodCallWithConditionalWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}