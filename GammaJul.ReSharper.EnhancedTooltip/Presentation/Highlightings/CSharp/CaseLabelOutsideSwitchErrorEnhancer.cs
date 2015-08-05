using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CaseLabelOutsideSwitchErrorEnhancer : CSharpHighlightingEnhancer<CaseLabelOutsideSwitchError> {

		protected override void AppendTooltip(CaseLabelOutsideSwitchError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Case label can be used in ");
			colorizer.AppendKeyword("switch");
			colorizer.AppendPlainText(" statement only");
		}
		
		public CaseLabelOutsideSwitchErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}