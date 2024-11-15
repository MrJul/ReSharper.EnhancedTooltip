using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class NullReferenceArgumentWarningEnhancer : CSharpHighlightingEnhancer<NullReferenceArgumentWarning> {

		protected override void AppendTooltip(NullReferenceArgumentWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Possible '");
			colorizer.AppendKeyword("null");
			colorizer.AppendPlainText("' reference argument for parameter '"); 
			colorizer.AppendParameterName(highlighting.Parameter.ShortName);
			colorizer.AppendPlainText("' in '");
			var declaredElement = new DeclaredElementInstance(highlighting.ParametersOwner, highlighting.Parameter.IdSubstitution);
			colorizer.AppendDeclaredElement(
				declaredElement.Element,
				declaredElement.Substitution,
				PresenterOptions.QualifiedName,
				highlighting.Node);
			colorizer.AppendPlainText("'");
		}
		
		public NullReferenceArgumentWarningEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}