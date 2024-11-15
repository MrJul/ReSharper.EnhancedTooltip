using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConvertIfStatementToSwitchStatementWarningEnhancer : CSharpHighlightingEnhancer<ConvertIfStatementToSwitchWarning> {

		protected override void AppendTooltip(ConvertIfStatementToSwitchWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert '");
			colorizer.AppendKeyword("if");
			colorizer.AppendPlainText("' statement to '");
			colorizer.AppendKeyword("switch");
			colorizer.AppendPlainText("' statement");
		}
		
		public ConvertIfStatementToSwitchStatementWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}