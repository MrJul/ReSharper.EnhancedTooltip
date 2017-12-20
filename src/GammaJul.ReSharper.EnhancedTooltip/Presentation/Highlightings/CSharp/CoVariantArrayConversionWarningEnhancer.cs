using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CoVariantArrayConversionWarningEnhancer : CSharpHighlightingEnhancer<CoVariantArrayConversionWarning> {

		protected override void AppendTooltip(CoVariantArrayConversionWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Co-variant array conversion from '");
			colorizer.AppendExpressionType(highlighting.ExpressionType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' to '");
			colorizer.AppendExpressionType(highlighting.TargetType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' can cause run-time exception on write operation");
		}

		public CoVariantArrayConversionWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}