using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class IncorrectTrueOrFalseOperatorTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectTrueOrFalseOperatorTypeError> {

		protected override void AppendTooltip(IncorrectTrueOrFalseOperatorTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The return type of operator ");
			colorizer.AppendKeyword("true");
			colorizer.AppendPlainText(" or ");
			colorizer.AppendKeyword("false");
			colorizer.AppendPlainText(" must be '");
			colorizer.AppendKeyword("bool");
			colorizer.AppendPlainText("'");
		}
		
		public IncorrectTrueOrFalseOperatorTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}