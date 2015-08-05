using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AbstractBaseMemberCallErrorEnhancer : CSharpHighlightingEnhancer<AbstractBaseMemberCallError> {

		protected override void AppendTooltip(AbstractBaseMemberCallError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot call an abstract base ");
			colorizer.AppendElementKind(highlighting.BaseMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.BaseMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember);
			colorizer.AppendPlainText("'");
		}

		public AbstractBaseMemberCallErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}