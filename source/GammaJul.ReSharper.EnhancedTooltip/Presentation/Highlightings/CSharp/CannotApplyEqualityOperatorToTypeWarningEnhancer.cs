using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotApplyEqualityOperatorToTypeWarningEnhancer : CSharpHighlightingEnhancer<CannotApplyEqualityOperatorToTypeWarning> {

		protected override void AppendTooltip(CannotApplyEqualityOperatorToTypeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot apply equality operator to type marked by '");
			colorizer.AppendClassName("CannotApplyEqualityOperator");
			colorizer.AppendPlainText("' attribute");
		}
		
		public CannotApplyEqualityOperatorToTypeWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}