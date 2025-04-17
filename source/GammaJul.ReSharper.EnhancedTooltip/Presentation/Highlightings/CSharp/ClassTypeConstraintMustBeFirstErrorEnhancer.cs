using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class ClassTypeConstraintMustBeFirstErrorEnhancer : CSharpHighlightingEnhancer<ClassTypeConstraintMustBeFirstError> {

		protected override void AppendTooltip(ClassTypeConstraintMustBeFirstError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The class type constraint '");
			colorizer.AppendExpressionType(highlighting.Constraint.Constraint, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' must come before any other constraints");
		}
		
		public ClassTypeConstraintMustBeFirstErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}