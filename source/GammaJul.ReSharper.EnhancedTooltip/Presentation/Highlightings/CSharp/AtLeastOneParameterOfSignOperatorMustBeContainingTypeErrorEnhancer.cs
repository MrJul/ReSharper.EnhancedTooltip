using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AtLeastOneParameterOfSignOperatorMustBeContainingTypeErrorEnhancer : CSharpHighlightingEnhancer<AtLeastOneParameterOfSignOperatorMustBeContainingTypeError> {

		protected override void AppendTooltip(AtLeastOneParameterOfSignOperatorMustBeContainingTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("At least one parameter should be '");
			colorizer.AppendDeclaredElement(highlighting.ContainingType, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.Declaration);
			colorizer.AppendPlainText("'");
		}
		
		public AtLeastOneParameterOfSignOperatorMustBeContainingTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}