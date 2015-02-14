using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectInitializerTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectInitializerTypeError> {

		protected override void AppendTooltip(IncorrectInitializerTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.SourceType.HasSameFullNameAs(highlighting.TargetType);

			colorizer.AppendPlainText("Cannot convert source type '");
			colorizer.AppendExpressionType(highlighting.SourceType, appendModuleName);
			colorizer.AppendPlainText("' to target type '");
			colorizer.AppendExpressionType(highlighting.TargetType, appendModuleName);
			colorizer.AppendPlainText("'");
		}
		
		public IncorrectInitializerTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}