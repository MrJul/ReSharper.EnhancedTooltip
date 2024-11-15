using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AnnotateNotNullTypeMemberWarningEnhancer : CSharpHighlightingEnhancer<AnnotateNotNullTypeMemberWarning> {
		
		protected override void AppendTooltip(AnnotateNotNullTypeMemberWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Annotate ");
			colorizer.AppendElementKind(highlighting.Declaration.DeclaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.AnnotationTypeElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("' with [");
			colorizer.AppendClassName(highlighting.IsContainerAnnotation ? "ItemNotNull" : "NotNull");
			colorizer.AppendPlainText("] attribute");
		}

		public AnnotateNotNullTypeMemberWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}