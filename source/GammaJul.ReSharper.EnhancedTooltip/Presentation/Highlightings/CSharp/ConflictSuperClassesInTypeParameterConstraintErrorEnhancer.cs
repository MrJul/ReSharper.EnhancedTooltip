using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConflictSuperClassesInTypeParameterConstraintErrorEnhancer : CSharpHighlightingEnhancer<ConflictSuperClassesInTypeParameterConstraintError> {

		protected override void AppendTooltip(ConflictSuperClassesInTypeParameterConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.ReferenceNode);
			colorizer.AppendPlainText("' inherits conflicting constraints '");
			colorizer.AppendExpressionType(highlighting.DeclaredType1, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendExpressionType(highlighting.DeclaredType2, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public ConflictSuperClassesInTypeParameterConstraintErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}