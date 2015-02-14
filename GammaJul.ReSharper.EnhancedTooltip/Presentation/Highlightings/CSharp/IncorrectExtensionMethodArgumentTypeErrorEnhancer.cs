using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectExtensionMethodArgumentTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectExtensionMethodArgumentTypeError> {

		protected override void AppendTooltip(IncorrectExtensionMethodArgumentTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.ArgumentType.HasSameFullNameAs(highlighting.ParameterType);

			colorizer.AppendPlainText("Cannot use conversion from '");
			colorizer.AppendExpressionType(highlighting.ArgumentType, appendModuleName);
			colorizer.AppendPlainText("' to '");
			colorizer.AppendExpressionType(highlighting.ParameterType, appendModuleName);
			colorizer.AppendPlainText("' in this context");
		}

		public IncorrectExtensionMethodArgumentTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}