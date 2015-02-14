using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectArrayElementTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArrayElementTypeError> {

		protected override void AppendTooltip(IncorrectArrayElementTypeError highlighting, CSharpColorizer colorizer) {
			IExpressionType sourceType = highlighting.InitializerExpression.GetExpressionType();
			bool appendModuleName = sourceType.HasSameFullNameAs(highlighting.TargetType);

			colorizer.AppendPlainText("Cannot convert expression of type '");
			colorizer.AppendExpressionType(sourceType, appendModuleName);
			colorizer.AppendPlainText("' to type '");
			colorizer.AppendExpressionType(highlighting.TargetType, appendModuleName);
			colorizer.AppendPlainText("'");
		}
		
		public IncorrectArrayElementTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}