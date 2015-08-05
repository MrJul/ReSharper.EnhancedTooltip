using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class AddressOfMarshalByRefObjectWarningEnhancer : CSharpHighlightingEnhancer<AddressOfMarshalByRefObjectWarning> {

		protected override void AppendTooltip(AddressOfMarshalByRefObjectWarning highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Passing '");
			colorizer.AppendDeclaredElement(highlighting.Field, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName);
			colorizer.AppendPlainText("' as ref or out or taking its address may cause a runtime exception because it is a field of a marshal-by-reference class");
		}
		
		public AddressOfMarshalByRefObjectWarningEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}