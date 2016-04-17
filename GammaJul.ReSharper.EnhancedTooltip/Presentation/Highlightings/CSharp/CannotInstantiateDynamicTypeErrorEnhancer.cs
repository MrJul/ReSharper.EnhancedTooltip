using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotInstantiateDynamicTypeErrorEnhancer : CSharpHighlightingEnhancer<CannotInstantiateDynamicTypeError> {

		protected override void AppendTooltip(CannotInstantiateDynamicTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot instantiate '");
			colorizer.AppendKeyword("dynamic");
			colorizer.AppendPlainText("' object");
		}
		
		public CannotInstantiateDynamicTypeErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}