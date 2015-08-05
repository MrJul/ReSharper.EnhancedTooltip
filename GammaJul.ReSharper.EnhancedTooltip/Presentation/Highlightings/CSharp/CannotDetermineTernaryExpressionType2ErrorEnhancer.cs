using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotDetermineTernaryExpressionType2ErrorEnhancer : CSharpHighlightingEnhancer<CannotDetermineTernaryExpressionType2Error> {

		protected override void AppendTooltip(CannotDetermineTernaryExpressionType2Error highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("There exist both implicit conversions from '");
			colorizer.AppendExpressionType(highlighting.ThenType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to '");
			colorizer.AppendExpressionType(highlighting.ElseType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' and from '");
			colorizer.AppendExpressionType(highlighting.ElseType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to '");
			colorizer.AppendExpressionType(highlighting.ThenType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotDetermineTernaryExpressionType2ErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}