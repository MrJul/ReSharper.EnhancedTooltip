using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotUseConstructorWithoutBracketsErrorEnhancer : CSharpHighlightingEnhancer<CannotUseConstructorWithoutBracketsError> {

		protected override void AppendTooltip(CannotUseConstructorWithoutBracketsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("A ");
			colorizer.AppendKeyword("new");
			colorizer.AppendPlainText(" expression requires (), [], or {} after type");
		}
		
		public CannotUseConstructorWithoutBracketsErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}