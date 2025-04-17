using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class ContextValueIsProvidedWarningEnhancer : CSharpHighlightingEnhancer<ContextValueIsProvidedWarning> {

		protected override void AppendTooltip(ContextValueIsProvidedWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Use value of '");
			colorizer.AppendExpressionType(highlighting.ExpressionType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' type provided by ");
			colorizer.AppendPlainText(highlighting.Suggestion.GetSourcePresentation());
		}

		public ContextValueIsProvidedWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}