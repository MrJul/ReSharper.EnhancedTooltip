using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
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
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}