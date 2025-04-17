using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotConstructVoidErrorEnhancer : CSharpHighlightingEnhancer<CannotConstructVoidError> {

		protected override void AppendTooltip(CannotConstructVoidError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot construct '");
			colorizer.AppendKeyword("void");
			colorizer.AppendPlainText("' type");
		}
		
		public CannotConstructVoidErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}