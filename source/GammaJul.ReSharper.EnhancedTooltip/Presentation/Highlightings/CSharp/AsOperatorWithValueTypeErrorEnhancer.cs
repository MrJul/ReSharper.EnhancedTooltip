using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AsOperatorWithValueTypeErrorEnhancer : CSharpHighlightingEnhancer<AsOperatorWithValueTypeError> {

		protected override void AppendTooltip(AsOperatorWithValueTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The ");
			colorizer.AppendKeyword("as");
			colorizer.AppendPlainText(" operator must be used with a reference or nullable type ('");
			colorizer.AppendExpressionType(highlighting.ValueType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' is a non-nullable value type)");
		}
		
		public AsOperatorWithValueTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}