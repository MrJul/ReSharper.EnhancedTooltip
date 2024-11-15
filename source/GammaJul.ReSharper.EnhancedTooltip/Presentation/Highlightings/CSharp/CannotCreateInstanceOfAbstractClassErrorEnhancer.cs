using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotCreateInstanceOfAbstractClassErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfAbstractClassError> {

		protected override void AppendTooltip(CannotCreateInstanceOfAbstractClassError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot create an instance of the ");
			colorizer.AppendKeyword("abstract");
			colorizer.AppendPlainText(" class '");
			colorizer.AppendDeclaredElement(highlighting.TypeElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.TypeElement.GetSingleDeclaration()?.FirstChild);
			colorizer.AppendPlainText("'");
		}
		
		public CannotCreateInstanceOfAbstractClassErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}
	}

}