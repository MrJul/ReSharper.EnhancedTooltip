using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectTrueOrFalseOperatorTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectTrueOrFalseOperatorTypeError> {

		protected override void AppendTooltip(IncorrectTrueOrFalseOperatorTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The return type of operator ");
			colorizer.AppendKeyword("true");
			colorizer.AppendPlainText(" or ");
			colorizer.AppendKeyword("false");
			colorizer.AppendPlainText(" must be '");
			colorizer.AppendKeyword("bool");
			colorizer.AppendPlainText("'");
		}
		
		public IncorrectTrueOrFalseOperatorTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}