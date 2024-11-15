using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CompareNonConstrainedGenericWithNullWarningEnhancer : CSharpHighlightingEnhancer<CompareNonConstrainedGenericWithNullWarning> {

		protected override void AppendTooltip(CompareNonConstrainedGenericWithNullWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Possible compare of value type with '");
			colorizer.AppendKeyword("null");
			colorizer.AppendPlainText("'");
		}

		public CompareNonConstrainedGenericWithNullWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}