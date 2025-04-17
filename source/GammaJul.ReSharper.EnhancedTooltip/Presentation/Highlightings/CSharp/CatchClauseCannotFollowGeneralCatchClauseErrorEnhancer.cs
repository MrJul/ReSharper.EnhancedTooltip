using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CatchClauseCannotFollowGeneralCatchClauseErrorEnhancer : CSharpHighlightingEnhancer<CatchClauseCannotFollowGeneralCatchClauseError> {

		protected override void AppendTooltip(CatchClauseCannotFollowGeneralCatchClauseError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Catch clauses cannot follow the general ");
			colorizer.AppendKeyword("catch");
			colorizer.AppendPlainText(" clause of a ");
			colorizer.AppendPlainText("try");
			colorizer.AppendPlainText(" statement");
		}
		
		public CatchClauseCannotFollowGeneralCatchClauseErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}