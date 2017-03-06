using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotHaveEventAccessorsInInterfacesErrorEnhancer : CSharpHighlightingEnhancer<CannotHaveEventAccessorsInInterfacesError> {

		protected override void AppendTooltip(CannotHaveEventAccessorsInInterfacesError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Event in interface cannot have ");
			colorizer.AppendAccessorName("add");
			colorizer.AppendPlainText(" or ");
			colorizer.AppendAccessorName("remove");
			colorizer.AppendPlainText(" accessors");
		}

		public CannotHaveEventAccessorsInInterfacesErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}