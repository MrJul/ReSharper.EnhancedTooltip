using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotChangeReturnTypeInOverrideErrorEnhancer : CSharpHighlightingEnhancer<CannotChangeReturnTypeInOverrideError> {

		protected override void AppendTooltip(CannotChangeReturnTypeInOverrideError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot change return type when overriding ");
			colorizer.AppendElementKind(highlighting.OverridenMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.OverridenMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember);
			colorizer.AppendPlainText("'");
		}
		
		public CannotChangeReturnTypeInOverrideErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}