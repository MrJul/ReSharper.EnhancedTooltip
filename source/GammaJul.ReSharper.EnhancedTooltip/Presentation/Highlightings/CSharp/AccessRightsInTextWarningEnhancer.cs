using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessRightsInTextWarningEnhancer : CSharpHighlightingEnhancer<AccessRightsInTextWarning> {

		protected override void AppendTooltip(AccessRightsInTextWarning highlighting, CSharpColorizer colorizer) {
			ResolveResultWithInfo resolveResult = highlighting.Reference.Resolve();
			if (resolveResult.DeclaredElement is not { } declaredElement)
				return;

			colorizer.AppendPlainText("Cannot access ");
			colorizer.AppendElementKind(declaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(declaredElement, resolveResult.Substitution, PresenterOptions.NameOnly, highlighting.Reference.GetTreeNode());
			colorizer.AppendPlainText("' here");
		}

		public AccessRightsInTextWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}