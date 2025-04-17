using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class IncorrectYieldReturnTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectYieldReturnTypeError> {

		protected override void AppendTooltip(IncorrectYieldReturnTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.ValueType.HasSameFullNameAs(highlighting.YieldType);

			colorizer.AppendPlainText("Cannot convert expression type '");
			colorizer.AppendExpressionType(highlighting.ValueType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to yield type '");
			colorizer.AppendExpressionType(highlighting.YieldType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}

		public IncorrectYieldReturnTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}