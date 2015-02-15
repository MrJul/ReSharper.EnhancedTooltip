using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AddressOfManagedTypeErrorEnhancer : CSharpHighlightingEnhancer<AddressOfManagedTypeError> {

		protected override void AppendTooltip(AddressOfManagedTypeError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot take the address of a variable of a managed type '");
			colorizer.AppendExpressionType(highlighting.OperandType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("'");
		}
		
		public AddressOfManagedTypeErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}