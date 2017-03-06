using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConstructorConstraintShouldBeLastErrorEnhancer : CSharpHighlightingEnhancer<ConstructorConstraintShouldBeLastError> {

		protected override void AppendTooltip(ConstructorConstraintShouldBeLastError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The ");
			colorizer.AppendKeyword("new");
			colorizer.AppendPlainText("() constraint must be the last constraint specified");
		}
		
		public ConstructorConstraintShouldBeLastErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}