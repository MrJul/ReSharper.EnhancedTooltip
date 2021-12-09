using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ArrayElementIsStaticClassErrorEnhancer : CSharpHighlightingEnhancer<ArrayElementIsStaticClassError> {

		protected override void AppendTooltip(ArrayElementIsStaticClassError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendExpressionType(highlighting.ScalarType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("': array elements cannot be of static type");
		}
		
		public ArrayElementIsStaticClassErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}