using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotUseConstructorWithoutBracketsErrorEnhancer : CSharpHighlightingEnhancer<CannotUseConstructorWithoutBracketsError> {

		protected override void AppendTooltip(CannotUseConstructorWithoutBracketsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("A ");
			colorizer.AppendKeyword("new");
			colorizer.AppendPlainText(" expression requires (), [], or {} after type");
		}
		
		public CannotUseConstructorWithoutBracketsErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}