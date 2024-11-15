using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AsOperatorWithUnconstrainedTypeParameterErrorEnhancer : CSharpHighlightingEnhancer<AsOperatorWithUnconstrainedTypeParameterError> {

		protected override void AppendTooltip(AsOperatorWithUnconstrainedTypeParameterError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The type parameter '");
			colorizer.AppendDeclaredElement(highlighting.TypeParameter, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, highlighting.TypeUsageNode);
			colorizer.AppendPlainText("' cannot be used with the '");
			colorizer.AppendKeyword("as");
			colorizer.AppendPlainText("' operator because it does not have a class type constraint nor a '");
			colorizer.AppendKeyword("class");
			colorizer.AppendPlainText("' constraint");
		}
		
		public AsOperatorWithUnconstrainedTypeParameterErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}