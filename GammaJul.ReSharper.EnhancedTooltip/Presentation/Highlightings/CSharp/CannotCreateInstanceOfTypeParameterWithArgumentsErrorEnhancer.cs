using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotCreateInstanceOfTypeParameterWithArgumentsErrorEnhancer : CSharpHighlightingEnhancer<CannotCreateInstanceOfTypeParameterWithArgumentsError> {

		protected override void AppendTooltip(CannotCreateInstanceOfTypeParameterWithArgumentsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot provide arguments when creating an instance of a type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}
		
		public CannotCreateInstanceOfTypeParameterWithArgumentsErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}