using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotHaveEventAccessorsInInterfacesErrorEnhancer : CSharpHighlightingEnhancer<CannotHaveEventAccessorsInInterfacesError> {

		protected override void AppendTooltip(CannotHaveEventAccessorsInInterfacesError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Event in interface cannot have ");
			colorizer.AppendAccessorName("add");
			colorizer.AppendPlainText(" or ");
			colorizer.AppendAccessorName("remove");
			colorizer.AppendPlainText(" accessors");
		}

		public CannotHaveEventAccessorsInInterfacesErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}