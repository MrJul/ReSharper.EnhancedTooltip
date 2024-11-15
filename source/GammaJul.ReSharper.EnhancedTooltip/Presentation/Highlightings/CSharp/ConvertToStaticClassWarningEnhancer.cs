using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConvertToStaticClassWarningEnhancer : CSharpHighlightingEnhancer<ConvertToStaticClassWarning> {

		protected override void AppendTooltip(ConvertToStaticClassWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Convert to '");
			colorizer.AppendKeyword("static");
			colorizer.AppendPlainText("' class");
		}

		public ConvertToStaticClassWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}