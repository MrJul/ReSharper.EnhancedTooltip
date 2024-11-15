using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotUseTypeArgumentWithElementErrorEnhancer : CSharpHighlightingEnhancer<CannotUseTypeArgumentWithElementError> {

		protected override void AppendTooltip(CannotUseTypeArgumentWithElementError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The ");
			colorizer.AppendElementKind(highlighting.Element);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.Element, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember, highlighting.TypeArgumentList);
			colorizer.AppendPlainText("' cannot be used with type arguments");
		}
		
		public CannotUseTypeArgumentWithElementErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}