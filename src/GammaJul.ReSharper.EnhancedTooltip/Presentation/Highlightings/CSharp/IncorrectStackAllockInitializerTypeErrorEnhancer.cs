using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectStackAllockInitializerTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectStackAllockInitializerTypeError> {

		protected override void AppendTooltip(IncorrectStackAllockInitializerTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.SourceType.HasSameFullNameAs(highlighting.TargetType);

			colorizer.AppendPlainText("Cannot convert source type '");
			colorizer.AppendExpressionType(highlighting.SourceType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to target type '");
			colorizer.AppendExpressionType(highlighting.TargetType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public IncorrectStackAllockInitializerTypeErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}