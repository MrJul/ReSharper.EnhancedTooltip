using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectArgumentKindErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArgumentKindError> {

		protected override void AppendTooltip(IncorrectArgumentKindError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Argument is '");
			colorizer.AppendParameterKind(highlighting.Argument.Kind);
			colorizer.AppendPlainText("' while parameter is declared as '");
			colorizer.AppendParameterKind(highlighting.ParameterKind);
			colorizer.AppendPlainText("'");
		}

		public IncorrectArgumentKindErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}