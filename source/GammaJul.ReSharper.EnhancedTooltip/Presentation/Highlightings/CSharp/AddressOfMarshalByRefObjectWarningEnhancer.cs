using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AddressOfMarshalByRefObjectWarningEnhancer : CSharpHighlightingEnhancer<AddressOfMarshalByRefObjectWarning> {

		protected override void AppendTooltip(AddressOfMarshalByRefObjectWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Passing '");
			colorizer.AppendDeclaredElement(highlighting.Field, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.ReferenceExpression);
			colorizer.AppendPlainText("' as '");
			colorizer.AppendKeyword(CSharpExpressionUtil.GetKindOfExplicitVariableReferenceCapture(highlighting.ReferenceExpression));
			colorizer.AppendPlainText("' argument may cause a runtime exception because it is a field of a marshal-by-reference class");
		}

		public AddressOfMarshalByRefObjectWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}