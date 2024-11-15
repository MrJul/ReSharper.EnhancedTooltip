using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotInheritFromSpecialTypeErrorEnhancer : CSharpHighlightingEnhancer<CannotInheritFromSpecialTypeError> {

		protected override void AppendTooltip(CannotInheritFromSpecialTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot derive from special class '");
			colorizer.AppendExpressionType(TypeFactory.CreateType(highlighting.TypeElement), false, PresenterOptions.QualifiedName);
			colorizer.AppendPlainText("'");
		}
		
		public CannotInheritFromSpecialTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}