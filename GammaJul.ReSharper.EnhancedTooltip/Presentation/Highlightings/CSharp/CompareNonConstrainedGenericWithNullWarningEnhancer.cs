using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CompareNonConstrainedGenericWithNullWarningEnhancer : CSharpHighlightingEnhancer<CompareNonConstrainedGenericWithNullWarning> {

		protected override void AppendTooltip(CompareNonConstrainedGenericWithNullWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Possible compare of value type with '");
			colorizer.AppendKeyword("null");
			colorizer.AppendPlainText("'");
		}

		public CompareNonConstrainedGenericWithNullWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}