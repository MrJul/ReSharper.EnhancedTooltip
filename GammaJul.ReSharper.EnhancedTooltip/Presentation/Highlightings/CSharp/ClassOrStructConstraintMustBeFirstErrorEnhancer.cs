using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ClassOrStructConstraintMustBeFirstErrorEnhancer : CSharpHighlightingEnhancer<ClassOrStructConstraintMustBeFirstError> {

		protected override void AppendTooltip(ClassOrStructConstraintMustBeFirstError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("The '");
			colorizer.AppendKeyword("class");
			colorizer.AppendPlainText("' or '");
			colorizer.AppendKeyword("struct");
			colorizer.AppendPlainText("' constraint must come before any other constraints");
		}
		
		public ClassOrStructConstraintMustBeFirstErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}