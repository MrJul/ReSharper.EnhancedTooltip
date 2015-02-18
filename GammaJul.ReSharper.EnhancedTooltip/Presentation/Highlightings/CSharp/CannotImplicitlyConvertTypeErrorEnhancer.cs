using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotImplicitlyConvertTypeErrorEnhancer : CSharpHighlightingEnhancer<CannotImplicitlyConvertTypeError> {

		protected override void AppendTooltip(CannotImplicitlyConvertTypeError highlighting, CSharpColorizer colorizer) {
			bool appendModuleName = highlighting.IExpressionType.HasSameFullNameAs(highlighting.TargetType);

			colorizer.AppendPlainText("Cannot implicitly convert type '");
			colorizer.AppendExpressionType(highlighting.IExpressionType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' to '");
			colorizer.AppendExpressionType(highlighting.TargetType, appendModuleName, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");

		}
		
		public CannotImplicitlyConvertTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}