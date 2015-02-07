using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectArgumentTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArgumentTypeError> {

		protected override void AppendTooltip(IncorrectArgumentTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = HaveSameFullName(highlighting.ArgumentType, highlighting.ParameterType);

			colorizer.AppendPlainText("Argument type '");
			colorizer.AppendType(highlighting.ArgumentType, appendModuleName);
			colorizer.AppendPlainText("' is not assignable to parameter type '");
			colorizer.AppendType(highlighting.ParameterType, appendModuleName);
			colorizer.AppendPlainText("'");
		}

		private static bool HaveSameFullName([NotNull] IExpressionType argumentType, [NotNull] IExpressionType parameterType) {
			return argumentType.GetLongPresentableName(CSharpLanguage.Instance) == parameterType.GetLongPresentableName(CSharpLanguage.Instance);
		}

		public IncorrectArgumentTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}