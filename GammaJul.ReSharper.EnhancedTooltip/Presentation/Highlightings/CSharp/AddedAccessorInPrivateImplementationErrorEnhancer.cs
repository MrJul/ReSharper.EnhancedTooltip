using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AddedAccessorInPrivateImplementationErrorEnhancer : CSharpHighlightingEnhancer<AddedAccessorInPrivateImplementationError> {

		protected override void AppendTooltip(AddedAccessorInPrivateImplementationError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("'");
			colorizer.AppendDeclaredElement(highlighting.Accessor, EmptySubstitution.INSTANCE, PresenterOptions.ForInterfaceMember);
			colorizer.AppendPlainText("' adds an accessor not found in interface member '");
			colorizer.AppendDeclaredElement(highlighting.SuperMember, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}

		public AddedAccessorInPrivateImplementationErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}