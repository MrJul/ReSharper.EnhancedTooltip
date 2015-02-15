using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotOverrideUnexistingAccessorErrorEnhancer : CSharpHighlightingEnhancer<CannotOverrideUnexistingAccessorError> {

		protected override void AppendTooltip(CannotOverrideUnexistingAccessorError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendDeclaredElement(highlighting.ExtraAccessor.DeclaredElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("': cannot override because '");
			colorizer.AppendDeclaredElement(highlighting.OverridenMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember);
			colorizer.AppendPlainText("' does not have an overridable ");
			colorizer.AppendAccessorKind(highlighting.ExtraAccessor.Kind);
			colorizer.AppendPlainText(" accessor");
		}
		
		public CannotOverrideUnexistingAccessorErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}