using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AssignNullToNotNullAttributeWarningEnhancer : CSharpHighlightingEnhancer<AssignNullToNotNullAttributeWarning> {

		protected override void AppendTooltip(AssignNullToNotNullAttributeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Possible '");
			colorizer.AppendKeyword("null");
			colorizer.AppendPlainText("' assignment to entity marked with '");
			colorizer.AppendClassName("NotNull");
			colorizer.AppendPlainText("' attribute");
		}
		
		public AssignNullToNotNullAttributeWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}