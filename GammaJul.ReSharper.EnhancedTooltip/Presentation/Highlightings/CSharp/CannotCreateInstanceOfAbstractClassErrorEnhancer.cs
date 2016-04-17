using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotCreateInstanceOfAbstractClassErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfAbstractClassError> {

		protected override void AppendTooltip(CannotCreateInstanceOfAbstractClassError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot create an instance of the ");
			colorizer.AppendKeyword("abstract");
			colorizer.AppendPlainText(" class '");
			colorizer.AppendDeclaredElement(highlighting.TypeElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName);
			colorizer.AppendPlainText("'");
		}
		
		public CannotCreateInstanceOfAbstractClassErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}