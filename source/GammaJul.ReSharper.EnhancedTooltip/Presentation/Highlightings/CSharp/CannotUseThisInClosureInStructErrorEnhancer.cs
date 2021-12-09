using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotUseThisInClosureInStructErrorEnhancer : CSharpHighlightingEnhancer<CannotUseThisInClosureInStructError> {

		protected override void AppendTooltip(CannotUseThisInClosureInStructError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Anonymous methods, lambda expressions, local functions and query expressions inside structs cannot access instance members of '");
			colorizer.AppendKeyword("this");
			colorizer.AppendPlainText("'. Consider copying '");
			colorizer.AppendKeyword("this");
			colorizer.AppendPlainText("' to a local variable outside the anonymous method, lambda expression, local function or query expression and using the local instead.");
		}
		
		public CannotUseThisInClosureInStructErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}