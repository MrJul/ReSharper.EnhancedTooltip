using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class BaseObjectGetHashCodeCallInGetHashCodeWarningEnhancer : CSharpHighlightingEnhancer<BaseObjectGetHashCodeCallInGetHashCodeWarning> {

		protected override void AppendTooltip(BaseObjectGetHashCodeCallInGetHashCodeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Overriden ");
			colorizer.AppendMethodName("GetHashCode");
			colorizer.AppendPlainText(" calls base '");
			colorizer.AppendClassName("Object");
			colorizer.AppendOperator(".");
			colorizer.AppendMethodName("GetHashCode");
			colorizer.AppendPlainText("()'");
		}
		
		public BaseObjectGetHashCodeCallInGetHashCodeWarningEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}