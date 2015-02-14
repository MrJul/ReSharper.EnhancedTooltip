using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectArgumentsErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArgumentsError> {

		protected override void AppendTooltip(IncorrectArgumentsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot resolve ");
			if (highlighting.UseElementType)
				colorizer.AppendElementKind(highlighting.Candidate);
			else
				colorizer.AppendPlainText("symbol");
			colorizer.AppendPlainText(" '");
			colorizer.AppendPlainText(highlighting.String1);
			colorizer.AppendPlainText("', candidates are:");
			colorizer.AppendCandidates(highlighting.Reference);
		}
		
		public IncorrectArgumentsErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}