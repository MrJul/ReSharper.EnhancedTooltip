using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotUseThisBaseInInitializerErrorEnhancer : CSharpHighlightingEnhancer<CannotUseThisBaseInInitializerError> {

		protected override void AppendTooltip(CannotUseThisBaseInInitializerError highlighting, CSharpColorizer colorizer) {
			string text = highlighting.Expression.GetText();
			
			colorizer.AppendPlainText("Cannot use '");
			if (CSharpLexer.IsKeyword(text))
				colorizer.AppendKeyword(text);
			else
				colorizer.AppendPlainText(text);
			colorizer.AppendPlainText("' in member initializer");
		}
		
		public CannotUseThisBaseInInitializerErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}