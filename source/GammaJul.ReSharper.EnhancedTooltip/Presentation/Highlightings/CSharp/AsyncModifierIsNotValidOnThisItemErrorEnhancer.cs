using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AsyncModifierIsNotValidOnThisItemErrorEnhancer : CSharpHighlightingEnhancer<AsyncModifierIsNotValidOnThisItemError> {

		protected override void AppendTooltip(AsyncModifierIsNotValidOnThisItemError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The modifier '");
			colorizer.AppendKeyword("async");
			colorizer.AppendPlainText("' is not valid for this item");
		}
		
		public AsyncModifierIsNotValidOnThisItemErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}