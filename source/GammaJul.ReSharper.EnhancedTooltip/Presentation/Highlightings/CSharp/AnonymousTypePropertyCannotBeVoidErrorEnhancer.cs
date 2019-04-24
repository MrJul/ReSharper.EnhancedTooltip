using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AnonymousTypePropertyCannotBeVoidErrorEnhancer : CSharpHighlightingEnhancer<AnonymousTypePropertyCannotBeVoidError> {

		protected override void AppendTooltip(AnonymousTypePropertyCannotBeVoidError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot assign '");
			colorizer.AppendKeyword("void");
			colorizer.AppendPlainText("' to anonymous type property");
		}
		
		public AnonymousTypePropertyCannotBeVoidErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}