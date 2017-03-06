using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertIfStatementToSwitchStatementWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToSwitchStatementWarning> {

		protected override void AppendTooltip(ConvertIfStatementToSwitchStatementWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert '");
			colorizer.AppendKeyword("if");
			colorizer.AppendPlainText("' statement to '");
			colorizer.AppendKeyword("switch");
			colorizer.AppendPlainText("' statement");
		}
		
		public ConvertIfStatementToSwitchStatementWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}