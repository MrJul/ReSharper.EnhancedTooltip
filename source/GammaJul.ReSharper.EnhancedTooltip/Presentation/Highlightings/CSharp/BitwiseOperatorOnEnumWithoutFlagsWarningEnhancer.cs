using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class BitwiseOperatorOnEnumWithoutFlagsWarningEnhancer : CSharpHighlightingEnhancer<BitwiseOperatorOnEnumWithoutFlagsWarning> {

		protected override void AppendTooltip(BitwiseOperatorOnEnumWithoutFlagsWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Bitwise operation on enum which is not marked by [");
			colorizer.AppendClassName("Flags");
			colorizer.AppendPlainText("] attribute");
		}
		
		public BitwiseOperatorOnEnumWithoutFlagsWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}