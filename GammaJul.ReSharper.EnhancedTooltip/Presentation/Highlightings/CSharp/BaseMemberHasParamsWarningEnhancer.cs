using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class BaseMemberHasParamsWarningEnhancer : CSharpHighlightingEnhancer<BaseMemberHasParamsWarning> {

		protected override void AppendTooltip(BaseMemberHasParamsWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Base ");
			colorizer.AppendElementKind(highlighting.BaseMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.BaseMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName);
			colorizer.AppendPlainText("' last parameter has '");
			colorizer.AppendKeyword("params");
			colorizer.AppendPlainText("' modifier");
		}
		
		public BaseMemberHasParamsWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}