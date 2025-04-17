using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class BaseObjectEqualsIsObjectEqualsWarningEnhancer : CSharpHighlightingEnhancer<BaseObjectEqualsIsObjectEqualsWarning> {

		protected override void AppendTooltip(BaseObjectEqualsIsObjectEqualsWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Call to '");
			colorizer.AppendKeyword("base");
			colorizer.AppendOperator(".");
			colorizer.AppendMethodName("Equals");
			colorizer.AppendPlainText("(...)' is reference equality");
		}
		
		public BaseObjectEqualsIsObjectEqualsWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}