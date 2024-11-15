using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConversionFromObjectErrorEnhancer : CSharpHighlightingEnhancer<ConversionFromObjectError> {

		protected override void AppendTooltip(ConversionFromObjectError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("User-defined conversion from '");
			colorizer.AppendNamespaceName("System");
			colorizer.AppendOperator(".");
			colorizer.AppendClassName("Object");
			colorizer.AppendPlainText("'");
		}

		public ConversionFromObjectErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}