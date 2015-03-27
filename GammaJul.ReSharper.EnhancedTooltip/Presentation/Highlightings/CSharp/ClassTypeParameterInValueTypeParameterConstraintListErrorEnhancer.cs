using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;
#if RS91
using JetBrains.ReSharper.Daemon.CSharp.Errors;
#else
using ClassTypeParameterInValueTypeParameterConstraintListError = JetBrains.ReSharper.Daemon.CSharp.Errors.ClassTypeParameterInValueTypeParameterConstrantListError;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ClassTypeParameterInValueTypeParameterConstraintListErrorEnhancer : CSharpHighlightingEnhancer<ClassTypeParameterInValueTypeParameterConstraintListError> {

		protected override void AppendTooltip(ClassTypeParameterInValueTypeParameterConstraintListError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Type parameter '");
			colorizer.AppendDeclaredElement(highlighting.Parameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' has the class-type constraint so '");
			colorizer.AppendDeclaredElement(highlighting.Parameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' cannot be used as a constraint for '");
			colorizer.AppendDeclaredElement(highlighting.Origin, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}
		
		public ClassTypeParameterInValueTypeParameterConstraintListErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}