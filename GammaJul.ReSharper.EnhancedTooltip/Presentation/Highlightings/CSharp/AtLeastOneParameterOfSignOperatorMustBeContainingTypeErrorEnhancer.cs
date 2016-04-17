using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AtLeastOneParameterOfSignOperatorMustBeContainingTypeErrorEnhancer : CSharpHighlightingEnhancer<AtLeastOneParameterOfSignOperatorMustBeContainingTypeError> {

		protected override void AppendTooltip(AtLeastOneParameterOfSignOperatorMustBeContainingTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("At least one parameter should be '");
			colorizer.AppendDeclaredElement(highlighting.ContainingType, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName);
			colorizer.AppendPlainText("'");
		}
		
		public AtLeastOneParameterOfSignOperatorMustBeContainingTypeErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}