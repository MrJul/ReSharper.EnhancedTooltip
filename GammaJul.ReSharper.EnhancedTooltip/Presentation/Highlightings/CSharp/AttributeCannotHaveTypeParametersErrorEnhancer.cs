using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AttributeCannotHaveTypeParametersErrorEnhancer : CSharpHighlightingEnhancer<AttributeCannotHaveTypeParametersError> {

		protected override void AppendTooltip(AttributeCannotHaveTypeParametersError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("A generic type cannot derive from '");
			colorizer.AppendNamespaceName("System");
			colorizer.AppendOperator(".");
			colorizer.AppendClassName("Attribute");
			colorizer.AppendPlainText("' because it is an attribute class");
		}
		
		public AttributeCannotHaveTypeParametersErrorEnhancer([NotNull] TextStyleHighlighterManager textStyleHighlighterManager, [NotNull] CodeAnnotationsCache codeAnnotationsCache)
			: base(textStyleHighlighterManager, codeAnnotationsCache) {
		}

	}

}