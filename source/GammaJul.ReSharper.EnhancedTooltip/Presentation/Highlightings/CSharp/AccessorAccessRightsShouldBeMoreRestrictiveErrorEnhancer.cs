using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AccessorAccessRightsShouldBeMoreRestrictiveErrorEnhancer : CSharpHighlightingEnhancer<AccessorAccessRightsShouldBeMoreRestrictiveError> {

		protected override void AppendTooltip(AccessorAccessRightsShouldBeMoreRestrictiveError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The accessibility modifier of the accessor must be more restrictive than the ");
			colorizer.AppendElementKind(highlighting.TypeMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.TypeMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.AccessorDeclaration);
			colorizer.AppendPlainText("'");
		}

		public AccessorAccessRightsShouldBeMoreRestrictiveErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}