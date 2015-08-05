using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotUseThisBaseInStaticContextErrorEnhancer : CSharpHighlightingEnhancer<CannotUseThisBaseInStaticContextError> {

		protected override void AppendTooltip(CannotUseThisBaseInStaticContextError highlighting, CSharpColorizer colorizer) {
			string text = highlighting.Expression.GetText();
			
			colorizer.AppendPlainText("Cannot use '");
			if (CSharpLexer.IsKeyword(text))
				colorizer.AppendKeyword(text);
			else
				colorizer.AppendPlainText(text);
			colorizer.AppendPlainText("' in static member");
		}
		
		public CannotUseThisBaseInStaticContextErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}