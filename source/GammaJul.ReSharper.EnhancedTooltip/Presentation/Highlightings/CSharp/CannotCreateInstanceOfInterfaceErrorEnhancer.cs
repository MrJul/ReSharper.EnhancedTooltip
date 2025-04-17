using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CannotCreateInstanceOfInterfaceErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfInterfaceError> {

		protected override void AppendTooltip(CannotCreateInstanceOfInterfaceError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot create an instance of the interface '");
			colorizer.AppendDeclaredElement(highlighting.TypeElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.ObjectCreationExpression);
			colorizer.AppendPlainText("'");
		}

		public CannotCreateInstanceOfInterfaceErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}