using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotOverrideNonExistingAccessorErrorEnhancer : CSharpHighlightingEnhancer<CannotOverrideNonExistingAccessorError> {

		protected override void AppendTooltip(CannotOverrideNonExistingAccessorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.TryAppendDeclaredElement(highlighting.ExtraAccessor, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.Declaration);
			colorizer.AppendPlainText("': cannot override because '");
			colorizer.AppendDeclaredElement(highlighting.OverriddenMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember, highlighting.Declaration);
			colorizer.AppendPlainText("' does not have an overridable ");
			colorizer.AppendAccessorKind(highlighting.ExtraAccessor.Kind);
			colorizer.AppendPlainText(" accessor");
		}

		public CannotOverrideNonExistingAccessorErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}