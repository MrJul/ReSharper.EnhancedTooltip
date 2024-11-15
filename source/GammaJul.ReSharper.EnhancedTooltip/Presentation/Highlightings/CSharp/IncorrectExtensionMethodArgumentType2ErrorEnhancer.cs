using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class IncorrectExtensionMethodArgumentType2ErrorEnhancer : CSharpHighlightingEnhancer<IncorrectExtensionMethodArgumentType2Error> {

		protected override void AppendTooltip(IncorrectExtensionMethodArgumentType2Error highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.ArgumentType.HasSameFullNameAs(highlighting.ParameterType);

			colorizer.AppendPlainText("Cannot convert instance argument type '");
			colorizer.AppendExpressionType(highlighting.ArgumentType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to '");
			colorizer.AppendExpressionType(highlighting.ParameterType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}

		public IncorrectExtensionMethodArgumentType2ErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}