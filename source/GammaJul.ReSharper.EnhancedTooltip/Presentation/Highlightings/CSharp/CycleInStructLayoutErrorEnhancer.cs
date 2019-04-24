using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CycleInStructLayoutErrorEnhancer : CSharpHighlightingEnhancer<CycleInStructLayoutError> {

		protected override void AppendTooltip(CycleInStructLayoutError highlighting, CSharpColorizer colorizer) {
			IClassMemberDeclaration declaration = highlighting.Declaration;
			ITypeMember declaredElement = declaration.DeclaredElement;
			if (declaredElement == null)
				return;

			colorizer.AppendPlainText(declaredElement.GetElementKindString(false, false, false, false, false).Capitalize());
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(declaredElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, declaration);
			colorizer.AppendPlainText("' of type '");
			colorizer.AppendExpressionType(highlighting.Type, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' causes a cycle in the struct layout");
		}
		
		public CycleInStructLayoutErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}