using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotCreateInstanceOfInterfaceErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfInterfaceError> {

		protected override void AppendTooltip(CannotCreateInstanceOfInterfaceError highlighting, CSharpColorizer colorizer) {
			IDeclaredElement declaredElement = highlighting.ReferenceName.Reference.Resolve().DeclaredElement;

			colorizer.AppendPlainText("Cannot create an instance of the interface '");
			if (declaredElement != null)
				colorizer.AppendDeclaredElement(declaredElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName);
			else
				colorizer.AppendInterfaceName(highlighting.ReferenceName.GetText());
			colorizer.AppendPlainText("'");
		}
		
		public CannotCreateInstanceOfInterfaceErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}