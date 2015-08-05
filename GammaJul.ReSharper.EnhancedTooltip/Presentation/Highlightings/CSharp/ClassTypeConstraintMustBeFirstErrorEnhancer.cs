using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ClassTypeConstraintMustBeFirstErrorEnhancer : CSharpHighlightingEnhancer<ClassTypeConstraintMustBeFirstError> {

		protected override void AppendTooltip(ClassTypeConstraintMustBeFirstError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The class type constraint '");
			colorizer.AppendExpressionType(highlighting.Constraint.Constraint, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' must come before any other constraints");
		}
		
		public ClassTypeConstraintMustBeFirstErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}