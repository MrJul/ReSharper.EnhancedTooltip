using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectArgumentTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArgumentTypeError> {

		protected override void AppendTooltip(IncorrectArgumentTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.ArgumentType.HasSameFullNameAs(highlighting.ParameterType);

			colorizer.AppendPlainText("Argument type '");
			colorizer.AppendExpressionType(highlighting.ArgumentType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' is not assignable to parameter type '");
			colorizer.AppendExpressionType(highlighting.ParameterType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}

		public IncorrectArgumentTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}