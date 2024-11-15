using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class IncorrectArgumentKindErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArgumentKindError> {

		protected override void AppendTooltip(IncorrectArgumentKindError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Argument is '");
			colorizer.AppendParameterKind(highlighting.Argument.Kind);
			colorizer.AppendPlainText("' while parameter is declared as '");
			colorizer.AppendParameterKind(highlighting.ParameterKind);
			colorizer.AppendPlainText("'");
		}

		public IncorrectArgumentKindErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}