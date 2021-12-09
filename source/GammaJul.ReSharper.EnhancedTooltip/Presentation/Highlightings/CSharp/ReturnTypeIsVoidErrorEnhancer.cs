using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ReturnTypeIsVoidErrorEnhancer : CSharpHighlightingEnhancer<ReturnTypeIsVoidError> {

		protected override void AppendTooltip(ReturnTypeIsVoidError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Return type is '");
			colorizer.AppendKeyword("void");
			colorizer.AppendPlainText("'");
		}

		public ReturnTypeIsVoidErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}