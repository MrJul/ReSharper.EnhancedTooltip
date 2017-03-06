using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ArglistParameterInAsyncErrorEnhancer : CSharpHighlightingEnhancer<ArglistParameterInAsyncError> {

		protected override void AppendTooltip(ArglistParameterInAsyncError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Async methods cannot have '");
			colorizer.AppendKeyword("__arglist");
			colorizer.AppendPlainText("' parameters");
		}

		public ArglistParameterInAsyncErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}