using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class CannotAccessExplicitImplementationErrorEnhancer : CSharpHighlightingEnhancer<CannotAccessExplicitImplementationError> {

		protected override void AppendTooltip(CannotAccessExplicitImplementationError highlighting, CSharpColorizer colorizer) {
			IExplicitImplementation explicitImplementation = highlighting.ExplicitImplementation;
			OverridableMemberInstance? resolvedMember = explicitImplementation.Resolve();

			colorizer.AppendPlainText("Cannot access explicit implementation of '");
			colorizer.AppendExpressionType(explicitImplementation.DeclaringType, false, PresenterOptions.NameOnly);
			colorizer.AppendOperator(".");
			if (resolvedMember is not null)
				colorizer.AppendDeclaredElement(resolvedMember.Element, resolvedMember.Substitution, PresenterOptions.NameOnly, highlighting.Reference.GetTreeNode());
			else
				colorizer.AppendPlainText(explicitImplementation.MemberName);
			colorizer.AppendPlainText("'");
		}
		
		public CannotAccessExplicitImplementationErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}