using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class BitwiseOperatorOnEnumWithoutFlagsWarningEnhancer : CSharpHighlightingEnhancer<BitwiseOperatorOnEnumWithoutFlagsWarning> {

		protected override void AppendTooltip(BitwiseOperatorOnEnumWithoutFlagsWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Bitwise operation on enum which is not marked by [");
			colorizer.AppendClassName("Flags");
			colorizer.AppendPlainText("] attribute");
		}
		
		public BitwiseOperatorOnEnumWithoutFlagsWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}