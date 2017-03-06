using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AllIndexersMustHaveSameNameErrorEnhancer : CSharpHighlightingEnhancer<AllIndexersMustHaveSameNameError> {
		
		protected override void AppendTooltip(AllIndexersMustHaveSameNameError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			colorizer.AppendClassName("IndexerName");
			colorizer.AppendPlainText("' attribute must be used with the same name on every indexer within a type");
		}

		public AllIndexersMustHaveSameNameErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}