using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotApplyEqualityOperatorToTypeWarningEnhancer : CSharpHighlightingEnhancer<CannotApplyEqualityOperatorToTypeWarning> {

		protected override void AppendTooltip(CannotApplyEqualityOperatorToTypeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply equality operator to type marked by '");
			colorizer.AppendClassName("CannotApplyEqualityOperator");
			colorizer.AppendPlainText("' attribute");
		}
		
		public CannotApplyEqualityOperatorToTypeWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}