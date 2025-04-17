using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class AddressOfManagedTypeErrorEnhancer : CSharpHighlightingEnhancer<AddressOfManagedTypeError> {

		protected override void AppendTooltip(AddressOfManagedTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot take the address of a variable of a managed type '");
			colorizer.AppendExpressionType(highlighting.OperandType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}
		
		public AddressOfManagedTypeErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}