using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CatchDoesNotExtendExceptionErrorEnhancer : CSharpHighlightingEnhancer<CatchDoesNotExtendExceptionError> {

		protected override void AppendTooltip(CatchDoesNotExtendExceptionError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Catch type should extend '");
			colorizer.AppendNamespaceName("System");
			colorizer.AppendOperator(".");
			colorizer.AppendClassName("Exception");
			colorizer.AppendPlainText("'");
		}
		
		public CatchDoesNotExtendExceptionErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}