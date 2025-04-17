using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotAwaitUnderQueryExpressionErrorEnhancer : CSharpHighlightingEnhancer<CannotAwaitUnderQueryExpressionError> {

		protected override void AppendTooltip(CannotAwaitUnderQueryExpressionError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' operator may only be used in a query expression within the first collection expression of the initial '");
			colorizer.AppendKeyword("from");
			colorizer.AppendPlainText("' clause or within the collection expression of a '");
			colorizer.AppendKeyword("join");
			colorizer.AppendPlainText("' clause");
		}
		
		public CannotAwaitUnderQueryExpressionErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}