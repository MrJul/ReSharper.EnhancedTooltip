using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectReturnTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectReturnTypeError> {

		protected override void AppendTooltip(IncorrectReturnTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.ValueType.HasSameFullNameAs(highlighting.ReturnType);

			colorizer.AppendPlainText("Cannot convert expression type '");
			colorizer.AppendExpressionType(highlighting.ValueType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to return type '");
			colorizer.AppendExpressionType(highlighting.ReturnType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}

		public IncorrectReturnTypeErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}