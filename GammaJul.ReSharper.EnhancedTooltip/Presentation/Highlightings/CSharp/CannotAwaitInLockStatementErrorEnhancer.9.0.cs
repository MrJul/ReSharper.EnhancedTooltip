using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotAwaitInLockStatementErrorEnhancer : CSharpHighlightingEnhancer<CannotAwaitInLockStatementError> {

		protected override void AppendTooltip(CannotAwaitInLockStatementError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot ");
			colorizer.AppendKeyword("await");
			colorizer.AppendPlainText(" in the body of a ");
			colorizer.AppendKeyword("lock");
			colorizer.AppendPlainText(" statement");
		}
		
		public CannotAwaitInLockStatementErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}