using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class IncorrectSignatureInDelegateCreationErrorEnhancer : CSharpHighlightingEnhancer<IncorrectSignatureInDelegateCreationError> {

		protected override void AppendTooltip(IncorrectSignatureInDelegateCreationError highlighting, CSharpColorizer colorizer) {
			var contextualNode = highlighting.Reference.GetTreeNode();

			colorizer.AppendPlainText("Expected a method with '");
			colorizer.AppendDeclaredElement(highlighting.TargetSignature, highlighting.TargetSubstitution, PresenterOptions.ElementTypeOnly, contextualNode);
			colorizer.AppendPlainText(highlighting.Reference.GetName());
			colorizer.AppendDeclaredElement(highlighting.TargetSignature, highlighting.TargetSubstitution, PresenterOptions.ParameterTypesOnly, contextualNode);
			colorizer.AppendPlainText("' signature");
		}

		public IncorrectSignatureInDelegateCreationErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}