using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessRightsForBothAccessorsErrorEnhancer : CSharpHighlightingEnhancer<AccessRightsForBothAccessorsError> {

		protected override void AppendTooltip(AccessRightsForBothAccessorsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot specify accessibility modifiers for both accessors of the ");
			colorizer.AppendElementKind(highlighting.TypeMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.TypeMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.AccessorDeclaration);
			colorizer.AppendPlainText("'");
		}

		public AccessRightsForBothAccessorsErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}