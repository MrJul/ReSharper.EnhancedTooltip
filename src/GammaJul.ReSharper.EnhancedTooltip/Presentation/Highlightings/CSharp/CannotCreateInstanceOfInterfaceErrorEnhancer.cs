using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotCreateInstanceOfInterfaceErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfInterfaceError> {

		protected override void AppendTooltip(CannotCreateInstanceOfInterfaceError highlighting, CSharpColorizer colorizer) {
			ResolveResultWithInfo resolveResult = highlighting.ReferenceName.Reference.Resolve();
			IDeclaredElement declaredElement = resolveResult.DeclaredElement;

			colorizer.AppendPlainText("Cannot create an instance of the interface '");
			if (declaredElement != null)
				colorizer.AppendDeclaredElement(declaredElement, resolveResult.Substitution, PresenterOptions.QualifiedName, highlighting.ReferenceName);
			else
				colorizer.AppendInterfaceName(highlighting.ReferenceName.GetText());
			colorizer.AppendPlainText("'");
		}
		
		public CannotCreateInstanceOfInterfaceErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}