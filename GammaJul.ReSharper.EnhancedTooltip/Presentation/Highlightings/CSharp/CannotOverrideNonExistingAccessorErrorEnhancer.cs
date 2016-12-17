using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotOverrideNonExistingAccessorErrorEnhancer : CSharpHighlightingEnhancer<CannotOverrideNonExistingAccessorError> {

		protected override void AppendTooltip(CannotOverrideNonExistingAccessorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.TryAppendDeclaredElement(highlighting.ExtraAccessor.DeclaredElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("': cannot override because '");
			colorizer.AppendDeclaredElement(highlighting.OverriddenMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember, highlighting.ExtraAccessor);
			colorizer.AppendPlainText("' does not have an overridable ");
			colorizer.AppendAccessorKind(highlighting.ExtraAccessor.Kind);
			colorizer.AppendPlainText(" accessor");
		}
		
		public CannotOverrideNonExistingAccessorErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}