using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ArglistParameterInDelegateErrorEnhancer : CSharpHighlightingEnhancer<ArglistParameterInDelegateError> {

		protected override void AppendTooltip(ArglistParameterInDelegateError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Delegates cannot have '");
			colorizer.AppendKeyword("__arglist");
			colorizer.AppendPlainText("' parameters");
		}

		public ArglistParameterInDelegateErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}