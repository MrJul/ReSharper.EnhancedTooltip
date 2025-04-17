using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class AnnotateCanBeNullTypeMemberWarningEnhancer : CSharpHighlightingEnhancer<AnnotateCanBeNullTypeMemberWarning> {
		
		protected override void AppendTooltip(AnnotateCanBeNullTypeMemberWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Annotate ");
			colorizer.AppendElementKind(highlighting.Declaration.DeclaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.AnnotationTypeElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("' with [");
			colorizer.AppendClassName(highlighting.IsContainerAnnotation ? "ItemCanBeNull" : "CanBeNull");
			colorizer.AppendPlainText("] attribute");
		}

		public AnnotateCanBeNullTypeMemberWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}