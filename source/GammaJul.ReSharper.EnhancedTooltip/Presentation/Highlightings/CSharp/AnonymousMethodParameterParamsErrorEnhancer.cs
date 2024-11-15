using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AnonymousMethodParameterParamsErrorEnhancer : CSharpHighlightingEnhancer<AnonymousMethodParameterParamsError> {

		protected override void AppendTooltip(AnonymousMethodParameterParamsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Anonymous method parameter cannot be '");
			colorizer.AppendKeyword("params");
			colorizer.AppendPlainText("'");
		}
		
		public AnonymousMethodParameterParamsErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}