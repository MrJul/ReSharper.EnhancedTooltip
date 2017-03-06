using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConversionToObjectErrorEnhancer : CSharpHighlightingEnhancer<ConversionToObjectError> {

		protected override void AppendTooltip(ConversionToObjectError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("User-defined conversion to '");
			colorizer.AppendNamespaceName("System");
			colorizer.AppendOperator(".");
			colorizer.AppendClassName("Object");
			colorizer.AppendPlainText("'");
		}

		public ConversionToObjectErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}