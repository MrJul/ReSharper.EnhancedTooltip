using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class ConstructorInitializerMissingErrorEnhancer : CSharpHighlightingEnhancer<ConstructorInitializerMissingError> {

		protected override void AppendTooltip(ConstructorInitializerMissingError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Base ");
			colorizer.AppendElementKind(highlighting.BaseType);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.BaseType, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.ConstructorDeclaration);
			colorizer.AppendPlainText("' doesn't contain parameterless constructor");
		}

		public ConstructorInitializerMissingErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}