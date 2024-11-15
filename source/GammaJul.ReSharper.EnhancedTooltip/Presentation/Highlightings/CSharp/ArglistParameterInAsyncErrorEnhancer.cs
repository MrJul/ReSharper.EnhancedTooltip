using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ArglistParameterInAsyncErrorEnhancer : CSharpHighlightingEnhancer<ArglistParameterInAsyncError> {

		protected override void AppendTooltip(ArglistParameterInAsyncError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Async methods cannot have '");
			colorizer.AppendKeyword("__arglist");
			colorizer.AppendPlainText("' parameters");
		}

		public ArglistParameterInAsyncErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}