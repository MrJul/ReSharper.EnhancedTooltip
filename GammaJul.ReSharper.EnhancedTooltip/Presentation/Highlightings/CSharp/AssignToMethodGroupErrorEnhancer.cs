using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AssignToMethodGroupErrorEnhancer : CSharpHighlightingEnhancer<AssignToMethodGroupError> {

		protected override void AppendTooltip(AssignToMethodGroupError highlighting, CSharpColorizer colorizer) {
			ResolveResultWithInfo resolveResult = highlighting.Expression.Reference.Resolve();
			IDeclaredElement declaredElement = resolveResult.DeclaredElement;
			if (declaredElement == null)
				return;

			colorizer.AppendPlainText("Cannot assign to '");
			colorizer.AppendDeclaredElement(declaredElement, resolveResult.Substitution, PresenterOptions.NameOnly, highlighting.Expression);
			colorizer.AppendPlainText("' because it is a 'method group'");
		}

		public AssignToMethodGroupErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}