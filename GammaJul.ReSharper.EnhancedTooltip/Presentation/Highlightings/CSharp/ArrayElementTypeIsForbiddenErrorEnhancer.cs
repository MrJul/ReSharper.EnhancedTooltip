using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ArrayElementTypeIsForbiddenErrorEnhancer : CSharpHighlightingEnhancer<ArrayElementTypeIsForbiddenError> {

		protected override void AppendTooltip(ArrayElementTypeIsForbiddenError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Array element type cannot be '");
			colorizer.AppendExpressionType(highlighting.ElementType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public ArrayElementTypeIsForbiddenErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}