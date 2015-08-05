using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotUseTypeArgumentWithElementErrorEnhancer : CSharpHighlightingEnhancer<CannotUseTypeArgumentWithElementError> {

		protected override void AppendTooltip(CannotUseTypeArgumentWithElementError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The ");
			colorizer.AppendElementKind(highlighting.Element);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.Element, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember);
			colorizer.AppendPlainText("' cannot be used with type arguments");
		}
		
		public CannotUseTypeArgumentWithElementErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}