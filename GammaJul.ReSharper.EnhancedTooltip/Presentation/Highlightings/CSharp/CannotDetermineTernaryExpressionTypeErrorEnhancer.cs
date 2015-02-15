using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotDetermineTernaryExpressionTypeErrorEnhancer : CSharpHighlightingEnhancer<CannotDetermineTernaryExpressionTypeError> {

		protected override void AppendTooltip(CannotDetermineTernaryExpressionTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("There is no implicit conversion between '");
			colorizer.AppendExpressionType(highlighting.ThenType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendExpressionType(highlighting.ElseType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotDetermineTernaryExpressionTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}