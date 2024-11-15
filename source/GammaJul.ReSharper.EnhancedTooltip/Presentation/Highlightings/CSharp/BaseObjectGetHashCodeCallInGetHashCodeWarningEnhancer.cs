using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
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
		
		public BaseObjectGetHashCodeCallInGetHashCodeWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}