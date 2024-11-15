using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotInheritFromSealedTypeErrorEnhancer : CSharpHighlightingEnhancer<CannotInheritFromSealedTypeError> {

		protected override void AppendTooltip(CannotInheritFromSealedTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot inherit from ");
			colorizer.AppendKeyword("sealed");
			colorizer.AppendPlainText(" ");
			colorizer.AppendElementKind(highlighting.TypeElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendExpressionType(TypeFactory.CreateType(highlighting.TypeElement), false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}
		
		public CannotInheritFromSealedTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}