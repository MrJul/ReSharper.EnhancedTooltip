using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ArrayElementTypeIsForbiddenErrorEnhancer : CSharpHighlightingEnhancer<ArrayElementTypeIsForbiddenError> {

		protected override void AppendTooltip(ArrayElementTypeIsForbiddenError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Array element type cannot be '");
			colorizer.AppendExpressionType(highlighting.ElementType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public ArrayElementTypeIsForbiddenErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}