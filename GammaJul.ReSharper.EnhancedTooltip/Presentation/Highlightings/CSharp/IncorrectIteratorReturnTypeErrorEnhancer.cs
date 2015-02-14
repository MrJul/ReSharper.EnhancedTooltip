using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectIteratorReturnTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectIteratorReturnTypeError> {

		protected override void AppendTooltip(IncorrectIteratorReturnTypeError highlighting, CSharpColorizer colorizer) {
			IFunction function = highlighting.Declaration.DeclaredElement;

			colorizer.AppendPlainText("The body of '");
			colorizer.AppendDeclaredElement(function, function.IdSubstitution, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' cannot be an iterator block because '");
			colorizer.AppendExpressionType(highlighting.ReturnType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' is not an iterator interface type");
		}
		
		public IncorrectIteratorReturnTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}