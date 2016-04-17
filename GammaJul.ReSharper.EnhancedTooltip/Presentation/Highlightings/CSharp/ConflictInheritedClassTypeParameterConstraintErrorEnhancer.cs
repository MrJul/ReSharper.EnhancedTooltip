using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConflictInheritedClassTypeParameterConstraintErrorEnhancer : CSharpHighlightingEnhancer<ConflictInheritedClassTypeParameterConstraintError> {

		protected override void AppendTooltip(ConflictInheritedClassTypeParameterConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' inherits conflicting constraints '");
			colorizer.AppendKeyword("class");
			colorizer.AppendPlainText("' and '");
			colorizer.AppendExpressionType(highlighting.ConflictingType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText("'");
		}
		
		public ConflictInheritedClassTypeParameterConstraintErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}