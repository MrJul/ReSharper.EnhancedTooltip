using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessRightsErrorEnhancer : CSharpHighlightingEnhancer<AccessRightsError> {

		protected override void AppendTooltip(AccessRightsError highlighting, CSharpColorizer colorizer) {
			IDeclaredElement declaredElement = highlighting.Reference.Resolve().DeclaredElement;
			if (declaredElement == null)
				return;

			colorizer.AppendPlainText("Cannot access ");
			colorizer.AppendAccessRights(declaredElement, false);
			colorizer.AppendPlainText(" ");
			colorizer.AppendElementKind(declaredElement);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(declaredElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' here");
		}

		public AccessRightsErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}