using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotImplementDynamicInterfaceErrorEnhancer : CSharpHighlightingEnhancer<CannotImplementDynamicInterfaceError> {

		protected override void AppendTooltip(CannotImplementDynamicInterfaceError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot implement a ");
			colorizer.AppendKeyword("dynamic");
			colorizer.AppendPlainText(" interface '");
			colorizer.AppendExpressionType(highlighting.SuperType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotImplementDynamicInterfaceErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}