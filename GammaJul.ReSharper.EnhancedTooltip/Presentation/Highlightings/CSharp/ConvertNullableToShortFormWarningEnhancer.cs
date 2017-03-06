using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConvertNullableToShortFormWarningEnhancer : CSharpHighlightingEnhancer<ConvertNullableToShortFormWarning> {

		protected override void AppendTooltip(ConvertNullableToShortFormWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Rewrite as '");
			colorizer.AppendExpressionType(highlighting.NullableType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}

		public ConvertNullableToShortFormWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}