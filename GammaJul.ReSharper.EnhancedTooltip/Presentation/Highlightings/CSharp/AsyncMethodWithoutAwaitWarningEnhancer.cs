using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AsyncMethodWithoutAwaitWarningEnhancer : CSharpHighlightingEnhancer<AsyncMethodWithoutAwaitWarning> {

		protected override void AppendTooltip(AsyncMethodWithoutAwaitWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("This ");
			colorizer.AppendKeyword("async");
			colorizer.AppendPlainText(" method lacks '");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' operators and will run synchronously. Consider using the '");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' operator to await non-blocking API calls, or '");
			colorizer.AppendKeyword("await ");
			colorizer.AppendClassName("Task");
			colorizer.AppendOperator(".");
			colorizer.AppendMethodName("Run");
			colorizer.AppendPlainText("(...)' to do CPU-bound work on a background thread");
		}
		
		public AsyncMethodWithoutAwaitWarningEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}