using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ArglistIteratorParameterErrorEnhancer : CSharpHighlightingEnhancer<ArglistIteratorParameterError> {

		protected override void AppendTooltip(ArglistIteratorParameterError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendKeyword("__arglist");
			colorizer.AppendPlainText("' is not allowed in the parameter list of iterators");
		}

		public ArglistIteratorParameterErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}