using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotUseThisInAnonymousMethodInStructErrorEnhancer : CSharpHighlightingEnhancer<CannotUseThisInAnonymousMethodInStructError> {

		protected override void AppendTooltip(CannotUseThisInAnonymousMethodInStructError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Anonymous methods inside structs cannot access instance members of '");
			colorizer.AppendKeyword("this");
			colorizer.AppendPlainText("'. Consider copying '");
			colorizer.AppendKeyword("this");
			colorizer.AppendPlainText("' to a local variable outside the anonymous method and using the local instead.");
		}
		
		public CannotUseThisInAnonymousMethodInStructErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}