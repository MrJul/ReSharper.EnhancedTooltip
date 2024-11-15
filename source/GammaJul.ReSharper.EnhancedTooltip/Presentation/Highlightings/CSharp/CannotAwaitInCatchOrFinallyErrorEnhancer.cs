using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotAwaitInCatchOrFinallyErrorEnhancer : CSharpHighlightingEnhancer<CannotAwaitInCatchOrFinallyError> {

		protected override void AppendTooltip(CannotAwaitInCatchOrFinallyError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot ");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText(" in the ");
			colorizer.AppendKeyword("catch");
			colorizer.AppendPlainText(" or ");
			colorizer.AppendKeyword("finally");
			colorizer.AppendPlainText(" clause of a ");
			colorizer.AppendKeyword("try");
			colorizer.AppendPlainText(" statement");
		}
		
		public CannotAwaitInCatchOrFinallyErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}