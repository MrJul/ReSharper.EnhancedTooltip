using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class AsyncMethodInvocationWithoutAwaitWarningEnhancer : CSharpHighlightingEnhancer<AsyncMethodInvocationWithoutAwaitWarning> {

		protected override void AppendTooltip(AsyncMethodInvocationWithoutAwaitWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the '");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText("' operator to the result of the call.");
		}
		
		public AsyncMethodInvocationWithoutAwaitWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}