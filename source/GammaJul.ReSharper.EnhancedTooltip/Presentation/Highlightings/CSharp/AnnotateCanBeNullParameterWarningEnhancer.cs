using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AnnotateCanBeNullParameterWarningEnhancer : CSharpHighlightingEnhancer<AnnotateCanBeNullParameterWarning> {
		
		protected override void AppendTooltip(AnnotateCanBeNullParameterWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Annotate ");
			colorizer.AppendElementKind(highlighting.Declaration.DeclaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.AnnotationTypeElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("' with [");
			colorizer.AppendClassName(highlighting.IsContainerAnnotation ? "ItemCanBeNull" : "CanBeNull");
			colorizer.AppendPlainText("] attribute");
		}

		public AnnotateCanBeNullParameterWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}