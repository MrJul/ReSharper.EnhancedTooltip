using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class IncorrectSignatureInDelegateCreationErrorEnhancer : CSharpHighlightingEnhancer<IncorrectSignatureInDelegateCreationError> {

		protected override void AppendTooltip(IncorrectSignatureInDelegateCreationError highlighting, CSharpColorizer colorizer) {
			IType returnType = highlighting.DelegateSubstitution.Apply(highlighting.CreatedDelegate.InvokeMethod.ReturnType);
			
			colorizer.AppendPlainText("Expected a method with '");
			colorizer.AppendExpressionType(returnType, false, PresenterOptions.FullWithoutParameterNames);
			colorizer.AppendPlainText(" ");
			colorizer.AppendPlainText(highlighting.Reference.GetName());
			colorizer.AppendDeclaredElement(highlighting.CreatedDelegate, highlighting.DelegateSubstitution, PresenterOptions.ParameterTypesOnly);
			colorizer.AppendPlainText("' signature");
		}

		public IncorrectSignatureInDelegateCreationErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}