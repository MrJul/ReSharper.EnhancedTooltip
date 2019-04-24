using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectIteratorReturnTypeErrorEnhancer : CSharpHighlightingEnhancer<IncorrectIteratorReturnTypeError> {

		protected override void AppendTooltip(IncorrectIteratorReturnTypeError highlighting, CSharpColorizer colorizer) {
			IDeclaredElement function = highlighting.Declaration.DeclaredElement;

			colorizer.AppendPlainText("The body of '");
			colorizer.TryAppendDeclaredElement(function, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("' cannot be an iterator block because '");
			colorizer.AppendExpressionType(highlighting.ReturnType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' is not an iterator interface type");
		}
		
		public IncorrectIteratorReturnTypeErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}