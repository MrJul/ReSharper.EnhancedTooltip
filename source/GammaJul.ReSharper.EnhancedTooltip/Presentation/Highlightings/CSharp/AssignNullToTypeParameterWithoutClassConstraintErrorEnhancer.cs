using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AssignNullToTypeParameterWithoutClassConstraintErrorEnhancer : CSharpHighlightingEnhancer<AssignNullToTypeParameterWithoutClassConstraintError> {

		protected override void AppendTooltip(AssignNullToTypeParameterWithoutClassConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot convert ");
			colorizer.AppendKeyword("null");
			colorizer.AppendPlainText(" to type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, highlighting.TypeParameter.IdSubstitution, PresenterOptions.NameOnly, highlighting.Expression);
			colorizer.AppendPlainText("' because it could be a value type. Consider using '");
			colorizer.AppendKeyword("default");
			colorizer.AppendPlainText("(");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, highlighting.TypeParameter.IdSubstitution, PresenterOptions.NameOnly, highlighting.Expression);
			colorizer.AppendPlainText(")' instead.");
		}

		public AssignNullToTypeParameterWithoutClassConstraintErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}