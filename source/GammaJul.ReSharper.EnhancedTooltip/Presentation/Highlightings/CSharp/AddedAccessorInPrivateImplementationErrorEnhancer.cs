using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class AddedAccessorInPrivateImplementationErrorEnhancer : CSharpHighlightingEnhancer<AddedAccessorInPrivateImplementationError> {

		protected override void AppendTooltip(AddedAccessorInPrivateImplementationError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendDeclaredElement(highlighting.Accessor, EmptySubstitution.INSTANCE, PresenterOptions.ForInterfaceMember, highlighting.InheritorDeclaration);
			colorizer.AppendPlainText("' adds an accessor not found in interface member '");
			colorizer.AppendDeclaredElement(highlighting.SuperMember, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.InheritorDeclaration);
			colorizer.AppendPlainText("'");
		}

		public AddedAccessorInPrivateImplementationErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}