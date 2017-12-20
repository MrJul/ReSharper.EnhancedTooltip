using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConflictInheritedTypeParameterConstraintErrorEnhancer : CSharpHighlightingEnhancer<ConflictInheritedTypeParameterConstraintError> {

		protected override void AppendTooltip(ConflictInheritedTypeParameterConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("' inherits conflicting constraints '");
			colorizer.AppendExpressionType(highlighting.Type1, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendExpressionType(highlighting.Type2, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public ConflictInheritedTypeParameterConstraintErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}