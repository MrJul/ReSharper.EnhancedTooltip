using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotImplicitlyConvertGotoCaseValueToGoverningTypeWarningEnhancer : CSharpHighlightingEnhancer<CannotImplicitlyConvertGotoCaseValueToGoverningTypeWarning> {

		protected override void AppendTooltip(CannotImplicitlyConvertGotoCaseValueToGoverningTypeWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			colorizer.AppendKeyword("goto case");
			colorizer.AppendPlainText("' value is not implicitly convertible to type '");
			colorizer.AppendExpressionType(highlighting.GoverningType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public CannotImplicitlyConvertGotoCaseValueToGoverningTypeWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}