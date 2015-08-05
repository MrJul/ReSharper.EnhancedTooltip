using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CollectionInitializerAppliedToNonCollectionErrorEnhancer : CSharpHighlightingEnhancer<CollectionInitializerAppliedToNonCollectionError> {

		protected override void AppendTooltip(CollectionInitializerAppliedToNonCollectionError highlighting, CSharpColorizer colorizer) {
			colorizer.AppendPlainText("Cannot initialize type '");
			colorizer.AppendExpressionType(highlighting.CreatedType, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' with a collection initializer because it does not implement '");
			colorizer.AppendNamespaceName("System");
			colorizer.AppendOperator(".");
			colorizer.AppendNamespaceName("Collections");
			colorizer.AppendOperator(".");
			colorizer.AppendInterfaceName("IEnumerable");
			colorizer.AppendPlainText("'");
		}

		public CollectionInitializerAppliedToNonCollectionErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsCache, highlighterIdProviderFactory) {
		}

	}

}