using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AnnotateNotNullParameterWarningEnhancer : CSharpHighlightingEnhancer<AnnotateNotNullParameterWarning> {
		
		protected override void AppendTooltip(AnnotateNotNullParameterWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Annotate ");
			colorizer.AppendElementKind(highlighting.Declaration.DeclaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.AnnotationTypeElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("' with [");
			colorizer.AppendClassName(highlighting.IsContainerAnnotation ? "ItemNotNull" : "NotNull");
			colorizer.AppendPlainText("] attribute");
		}

		public AnnotateNotNullParameterWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}