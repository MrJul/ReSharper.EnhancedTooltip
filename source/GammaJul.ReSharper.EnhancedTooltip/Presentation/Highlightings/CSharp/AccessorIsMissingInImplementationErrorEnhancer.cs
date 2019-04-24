using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AccessorIsMissingInImplementationErrorEnhancer : CSharpHighlightingEnhancer<AccessorIsMissingInImplementationError> {

		protected override void AppendTooltip(AccessorIsMissingInImplementationError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Interface implementation '");
			colorizer.TryAppendDeclaredElement(highlighting.InheritorDeclaration.DeclaredElement, EmptySubstitution.INSTANCE, PresenterOptions.ForInterfaceMember, highlighting.InheritorDeclaration);
			colorizer.AppendPlainText("' is missing accessor '");
			colorizer.AppendDeclaredElement(highlighting.MissedAccessor, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.InheritorDeclaration);
			colorizer.AppendPlainText("' implementation");
		}

		public AccessorIsMissingInImplementationErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}