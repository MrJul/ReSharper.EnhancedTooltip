using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessToRefOutParameterFromAnonymousMethodErrorEnhancer : CSharpHighlightingEnhancer<AccessToRefOutParameterFromAnonymousMethodError> {

		protected override void AppendTooltip(AccessToRefOutParameterFromAnonymousMethodError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot use '");
			colorizer.AppendKeyword("ref");
			colorizer.AppendPlainText("' or '");
			colorizer.AppendKeyword("out");
			colorizer.AppendPlainText("' parameter '");
			colorizer.AppendDeclaredElement(highlighting.Parameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' inside an anonymous method body");
		}

		public AccessToRefOutParameterFromAnonymousMethodErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}