using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class BaseObjectEqualsIsObjectEqualsWarningEnhancer : CSharpHighlightingEnhancer<BaseObjectEqualsIsObjectEqualsWarning> {

		protected override void AppendTooltip(BaseObjectEqualsIsObjectEqualsWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Call to '");
			colorizer.AppendKeyword("base");
			colorizer.AppendOperator(".");
			colorizer.AppendMethodName("Equals");
			colorizer.AppendPlainText("(...)' is reference equality");
		}
		
		public BaseObjectEqualsIsObjectEqualsWarningEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}