using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class AttributeNameExpectedErrorEnhancer : CSharpHighlightingEnhancer<AttributeNameExpectedError> {

		protected override void AppendTooltip(AttributeNameExpectedError highlighting, CSharpColorizer colorizer) {
			ResolveResultWithInfo resolveResult = highlighting.Reference.Resolve();
			if (resolveResult.DeclaredElement is not { } declaredElement)
				return;

			colorizer.AppendPlainText("'");
			colorizer.AppendDeclaredElement(declaredElement, resolveResult.Substitution, PresenterOptions.NameOnly, highlighting.Reference.GetTreeNode());
			colorizer.AppendPlainText("' is not an attribute");
		}

		public AttributeNameExpectedErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}