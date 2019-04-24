using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ConstructorInitializerMissingErrorEnhancer : CSharpHighlightingEnhancer<ConstructorInitializerMissingError> {

		protected override void AppendTooltip(ConstructorInitializerMissingError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Base class '");
			colorizer.AppendDeclaredElement(highlighting.BaseClass, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.ConstructorDeclaration);
			colorizer.AppendPlainText("' doesn't contain parameterless constructor");
		}

		public ConstructorInitializerMissingErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}