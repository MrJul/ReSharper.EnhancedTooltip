using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotConstructVoidErrorEnhancer : CSharpHighlightingEnhancer<CannotConstructVoidError> {

		protected override void AppendTooltip(CannotConstructVoidError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot construct '");
			colorizer.AppendKeyword("void");
			colorizer.AppendPlainText("' type");
		}
		
		public CannotConstructVoidErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}