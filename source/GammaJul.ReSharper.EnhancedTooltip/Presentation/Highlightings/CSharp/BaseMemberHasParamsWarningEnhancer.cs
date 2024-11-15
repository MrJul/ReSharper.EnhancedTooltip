using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class BaseMemberHasParamsWarningEnhancer : CSharpHighlightingEnhancer<BaseMemberHasParamsWarning> {

		protected override void AppendTooltip(BaseMemberHasParamsWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Base ");
			colorizer.AppendElementKind(highlighting.BaseMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.BaseMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.Declaration);
			colorizer.AppendPlainText("' last parameter has '");
			colorizer.AppendKeyword("params");
			colorizer.AppendPlainText("' modifier");
		}
		
		public BaseMemberHasParamsWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}