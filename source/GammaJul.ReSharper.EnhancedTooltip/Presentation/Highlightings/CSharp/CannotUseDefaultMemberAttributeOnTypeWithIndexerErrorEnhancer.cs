using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotUseDefaultMemberAttributeOnTypeWithIndexerErrorEnhancer : CSharpHighlightingEnhancer<CannotUseDefaultMemberAttributeOnTypeWithIndexerError> {

		protected override void AppendTooltip(CannotUseDefaultMemberAttributeOnTypeWithIndexerError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot specify the '");
			colorizer.AppendClassName("DefaultMember");
			colorizer.AppendPlainText("' attribute on type containing an indexer");
		}
		
		public CannotUseDefaultMemberAttributeOnTypeWithIndexerErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}