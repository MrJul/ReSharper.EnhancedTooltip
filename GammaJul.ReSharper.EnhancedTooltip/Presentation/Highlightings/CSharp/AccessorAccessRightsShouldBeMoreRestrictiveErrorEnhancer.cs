using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessorAccessRightsShouldBeMoreRestrictiveErrorEnhancer : CSharpHighlightingEnhancer<AccessorAccessRightsShouldBeMoreRestrictiveError> {

		protected override void AppendTooltip(AccessorAccessRightsShouldBeMoreRestrictiveError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The accessibility modifier of the accessor must be more restrictive than the ");
			colorizer.AppendElementKind(highlighting.TypeMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.TypeMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName);
			colorizer.AppendPlainText("'");
		}

		public AccessorAccessRightsShouldBeMoreRestrictiveErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}