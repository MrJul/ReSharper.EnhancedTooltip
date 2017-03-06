using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfStatementToReturnConditionalWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToReturnConditionalWarning> {

		protected override void AppendTooltip(ConvertIfStatementToReturnConditionalWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to '");
			colorizer.AppendKeyword("return");
			colorizer.AppendPlainText("' statement");
		}
		
		public ConvertIfStatementToReturnConditionalWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}