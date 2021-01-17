using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
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
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}