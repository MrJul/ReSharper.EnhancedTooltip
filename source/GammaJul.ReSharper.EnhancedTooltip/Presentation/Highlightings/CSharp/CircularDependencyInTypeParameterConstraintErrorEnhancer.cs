using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CircularDependencyInTypeParameterConstraintErrorEnhancer : CSharpHighlightingEnhancer<CircularDependencyInTypeParameterConstraintError> {

		protected override void AppendTooltip(CircularDependencyInTypeParameterConstraintError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Circular constraint dependency involving '");
			colorizer.AppendDeclaredElement(highlighting.Origin, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.TypeUsageNode);
			colorizer.AppendPlainText("' and '");
			colorizer.AppendDeclaredElement(highlighting.Parameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.TypeUsageNode);
			colorizer.AppendPlainText("'");
		}
		
		public CircularDependencyInTypeParameterConstraintErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}