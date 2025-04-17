using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class IncorrectArgumentsErrorEnhancer : CSharpHighlightingEnhancer<IncorrectArgumentsError> {

		protected override void AppendTooltip(IncorrectArgumentsError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot resolve ");
			if (highlighting.UseElementType)
				colorizer.AppendElementKind(highlighting.Candidate);
			else
				colorizer.AppendPlainText("symbol");
			colorizer.AppendPlainText(" '");
			colorizer.AppendPlainText(highlighting.Candidate.ShortName);
			colorizer.AppendPlainText("', candidates are:");
			colorizer.AppendCandidates(highlighting.Reference);
		}
		
		public IncorrectArgumentsErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}