using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class IncorrectPropertyAccessorNameErrorEnhancer : CSharpHighlightingEnhancer<IncorrectPropertyAccessorNameError> {

		protected override void AppendTooltip(IncorrectPropertyAccessorNameError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendKeyword("get");
			colorizer.AppendPlainText("', '");
			colorizer.AppendKeyword("set");
			colorizer.AppendPlainText("' or '");
			colorizer.AppendKeyword("init");
			colorizer.AppendPlainText("' expected");
		}

		public IncorrectPropertyAccessorNameErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}