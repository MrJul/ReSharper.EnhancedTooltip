using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AwaitIdentifierInAsyncMethodErrorEnhancer : CSharpHighlightingEnhancer<AwaitIdentifierInAsyncMethodError> {

		protected override void AppendTooltip(AwaitIdentifierInAsyncMethodError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' cannot be used as an identifier within an ");
			colorizer.AppendKeyword("async");
			colorizer.AppendPlainText(" method or lambda expression");
		}
		
		public AwaitIdentifierInAsyncMethodErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}