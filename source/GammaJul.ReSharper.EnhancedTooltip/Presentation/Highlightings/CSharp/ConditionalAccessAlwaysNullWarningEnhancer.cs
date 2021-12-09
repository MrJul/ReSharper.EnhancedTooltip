using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConditionalAccessAlwaysNullWarningEnhancer : CSharpHighlightingEnhancer<ConditionalAccessAlwaysNullWarning> {

		protected override void AppendTooltip(ConditionalAccessAlwaysNullWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Conditional access qualifier expression is known to be ");
			colorizer.AppendKeyword("null");
		}

		public ConditionalAccessAlwaysNullWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}