using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AwaitNotUnderAsyncMethodErrorEnhancer : CSharpHighlightingEnhancer<AwaitNotUnderAsyncMethodError> {

		protected override void AppendTooltip(AwaitNotUnderAsyncMethodError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			switch (highlighting.AwaitOwner) {
				case IAwaitExpression _:
					colorizer.AppendKeyword("await");
					colorizer.AppendPlainText("' expression");
					break;
				case IForeachStatement foreachStatement:
					if (foreachStatement.IsAwait) {
						colorizer.AppendKeyword("await foreach");
						colorizer.AppendPlainText("' statement");
					}
					break;
			}
			colorizer.AppendPlainText(" can only be used in a method or lambda marked with the '");
			colorizer.AppendKeyword("async");
			colorizer.AppendPlainText("' modifier");
		}
		
		public AwaitNotUnderAsyncMethodErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}