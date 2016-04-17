using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class BothRefValueAndClassConstraintErrorEnhancer : CSharpHighlightingEnhancer<BothRefValueAndClassConstraintError> {

		protected override void AppendTooltip(BothRefValueAndClassConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot specify both a constraint class and the '");
			colorizer.AppendKeyword("class");
			colorizer.AppendPlainText("' or '");
			colorizer.AppendKeyword("struct");
			colorizer.AppendPlainText("' constraint");
		}
		
		public BothRefValueAndClassConstraintErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}