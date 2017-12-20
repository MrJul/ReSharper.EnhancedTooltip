using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AnonymousMethodParameterParamsErrorEnhancer : CSharpHighlightingEnhancer<AnonymousMethodParameterParamsError> {

		protected override void AppendTooltip(AnonymousMethodParameterParamsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Anonymous method parameter cannot be '");
			colorizer.AppendKeyword("params");
			colorizer.AppendPlainText("'");
		}
		
		public AnonymousMethodParameterParamsErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}