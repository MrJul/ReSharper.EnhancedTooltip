using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class ConditionalAccessAlwaysNotNullWarningEnhancer : CSharpHighlightingEnhancer<ConditionalAccessAlwaysNotNullWarning> {

		protected override void AppendTooltip(ConditionalAccessAlwaysNotNullWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Conditional access qualifier expression is known to be not ");
			colorizer.AppendKeyword("null");
		}

		public ConditionalAccessAlwaysNotNullWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}