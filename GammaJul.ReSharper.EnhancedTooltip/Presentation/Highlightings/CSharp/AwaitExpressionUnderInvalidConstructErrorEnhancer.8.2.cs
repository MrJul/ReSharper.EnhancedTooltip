using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AwaitExpressionUnderInvalidConstructErrorEnhancer : CSharpHighlightingEnhancer<AwaitExpressionUnderInvalidConstructError> {

		protected override void AppendTooltip(AwaitExpressionUnderInvalidConstructError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' operator cannot occur inside a ");
			colorizer.AppendKeyword("catch");
			colorizer.AppendPlainText(" or ");
			colorizer.AppendKeyword("finally");
			colorizer.AppendPlainText(" block of a ");
			colorizer.AppendKeyword("try");
			colorizer.AppendPlainText("-statement, inside the block of a ");
			colorizer.AppendKeyword("lock");
			colorizer.AppendPlainText("-statement, or in an ");
			colorizer.AppendKeyword("unsafe");
			colorizer.AppendPlainText(" context");
		}
		
		public AwaitExpressionUnderInvalidConstructErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}