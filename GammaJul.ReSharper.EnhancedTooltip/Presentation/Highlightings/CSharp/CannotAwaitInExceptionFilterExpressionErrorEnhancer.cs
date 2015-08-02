using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotAwaitInExceptionFilterExpressionErrorEnhancer : CSharpHighlightingEnhancer<CannotAwaitInExceptionFilterExpressionError> {

		protected override void AppendTooltip(CannotAwaitInExceptionFilterExpressionError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot ");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText(" in the filter expression of a ");
			colorizer.AppendKeyword("catch");
			colorizer.AppendPlainText(" clause");
		}
		
		public CannotAwaitInExceptionFilterExpressionErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}