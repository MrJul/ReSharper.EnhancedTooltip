using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class ClassTypeParameterInValueTypeParameterConstraintListErrorEnhancer : CSharpHighlightingEnhancer<ClassTypeParameterInValueTypeParameterConstraintListError> {

		protected override void AppendTooltip(ClassTypeParameterInValueTypeParameterConstraintListError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Type parameter '");
			colorizer.AppendDeclaredElement(highlighting.Parameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.TypeUsageNode);
			colorizer.AppendPlainText("' has the class-type constraint so '");
			colorizer.AppendDeclaredElement(highlighting.Parameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.TypeUsageNode);
			colorizer.AppendPlainText("' cannot be used as a constraint for '");
			colorizer.AppendDeclaredElement(highlighting.Origin, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.TypeUsageNode);
			colorizer.AppendPlainText("'");
		}
		
		public ClassTypeParameterInValueTypeParameterConstraintListErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}