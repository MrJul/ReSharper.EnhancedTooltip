using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AssignVoidToRangeVariableErrorEnhancer : CSharpHighlightingEnhancer<AssignVoidToRangeVariableError> {

		protected override void AppendTooltip(AssignVoidToRangeVariableError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot assign ");
			colorizer.AppendKeyword("void");
			colorizer.AppendPlainText(" to a range variable");
		}
		
		public AssignVoidToRangeVariableErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}