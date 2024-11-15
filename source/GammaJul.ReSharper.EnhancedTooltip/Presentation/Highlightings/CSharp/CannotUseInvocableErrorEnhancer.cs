using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotUseInvocableErrorEnhancer : CSharpHighlightingEnhancer<CannotUseInvocableError> {

		protected override void AppendTooltip(CannotUseInvocableError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Use of  ");
			if (highlighting.UseElementType)
				colorizer.AppendElementKind(highlighting.Candidate);
			else
				colorizer.AppendPlainText("symbol");
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.Candidate, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Reference.GetTreeNode());
			colorizer.AppendPlainText("' without ()");
		}
		
		public CannotUseInvocableErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}