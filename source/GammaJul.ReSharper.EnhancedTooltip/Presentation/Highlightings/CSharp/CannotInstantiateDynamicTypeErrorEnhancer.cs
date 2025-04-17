using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotInstantiateDynamicTypeErrorEnhancer : CSharpHighlightingEnhancer<CannotInstantiateDynamicTypeError> {

		protected override void AppendTooltip(CannotInstantiateDynamicTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot instantiate '");
			colorizer.AppendKeyword("dynamic");
			colorizer.AppendPlainText("' object");
		}
		
		public CannotInstantiateDynamicTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}