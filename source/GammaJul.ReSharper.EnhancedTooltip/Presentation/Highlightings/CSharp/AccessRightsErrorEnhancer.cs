using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessRightsErrorEnhancer : CSharpHighlightingEnhancer<AccessRightsError> {

		protected override void AppendTooltip(AccessRightsError highlighting, CSharpColorizer colorizer) {
			ResolveResultWithInfo resolveResult = highlighting.Reference.Resolve();
			if (resolveResult.DeclaredElement is not { } declaredElement)
				return;

			colorizer.AppendPlainText("Cannot access ");
			colorizer.AppendAccessRights(declaredElement, false);
			colorizer.AppendPlainText(" ");
			colorizer.AppendElementKind(declaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(declaredElement, resolveResult.Substitution, PresenterOptions.NameOnly, highlighting.Reference.GetTreeNode());
			colorizer.AppendPlainText("' here");
		}

		public AccessRightsErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}