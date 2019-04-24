using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AssignNullToNotNullAttributeWarningEnhancer : CSharpHighlightingEnhancer<AssignNullToNotNullAttributeWarning> {

		protected override void AppendTooltip(AssignNullToNotNullAttributeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Possible '");
			colorizer.AppendKeyword("null");
			colorizer.AppendPlainText("' assignment to entity marked with '");
			colorizer.AppendClassName("NotNull");
			colorizer.AppendPlainText("' attribute");
		}
		
		public AssignNullToNotNullAttributeWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}