using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AwaitExpressionNotUnderAsyncMethodErrorEnhancer : CSharpHighlightingEnhancer<AwaitExpressionNotUnderAsyncMethodError> {

		protected override void AppendTooltip(AwaitExpressionNotUnderAsyncMethodError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' operator can only be used in a method or lambda marked with the '");
			colorizer.AppendKeyword("async");
			colorizer.AppendPlainText("' modifier");
		}
		
		public AwaitExpressionNotUnderAsyncMethodErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}