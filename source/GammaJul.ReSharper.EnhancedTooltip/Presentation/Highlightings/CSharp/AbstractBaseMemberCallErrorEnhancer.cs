using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AbstractBaseMemberCallErrorEnhancer : CSharpHighlightingEnhancer<AbstractBaseMemberCallError> {

		protected override void AppendTooltip(AbstractBaseMemberCallError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot call an abstract base ");
			colorizer.AppendElementKind(highlighting.BaseMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.BaseMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember, highlighting.Expression);
			colorizer.AppendPlainText("'");
		}

		public AbstractBaseMemberCallErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}