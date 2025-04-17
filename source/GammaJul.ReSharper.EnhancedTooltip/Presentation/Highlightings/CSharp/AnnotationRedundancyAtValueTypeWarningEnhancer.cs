using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class AnnotationRedundancyAtValueTypeWarningEnhancer : CSharpHighlightingEnhancer<AnnotationRedundancyAtValueTypeWarning> {

		protected override void AppendTooltip(AnnotationRedundancyAtValueTypeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Applying nullness annotation to ");
			if (highlighting.IsVoid) {
				colorizer.AppendPlainText("'");
				colorizer.AppendKeyword("void");
				colorizer.AppendPlainText("'");
			}
			else
				colorizer.AppendPlainText("value");
			colorizer.AppendPlainText(" type is meaningless");
		}
		
		public AnnotationRedundancyAtValueTypeWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}