using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectForeachVariableTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectForeachVariableTypeError> {

		protected override void AppendTooltip(IncorrectForeachVariableTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.ElementType.HasSameFullNameAs(highlighting.IteratorType);

			colorizer.AppendPlainText("Cannot convert element type '");
			colorizer.AppendExpressionType(highlighting.ElementType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to iterator type '");
			colorizer.AppendExpressionType(highlighting.IteratorType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}

		public IncorrectForeachVariableTypeErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}