using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	internal sealed class CycleInStructLayoutErrorEnhancer : CSharpHighlightingEnhancer<CycleInStructLayoutError> {

		protected override void AppendTooltip(CycleInStructLayoutError highlighting, CSharpColorizer colorizer) {
			IClassMemberDeclaration? declaration = highlighting.Declaration as IClassMemberDeclaration;
			if (declaration?.DeclaredElement is not { } declaredElement)
				return;

			colorizer.AppendPlainText(declaredElement.GetElementKindString(false, false, false, false, false).Capitalize());
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(declaredElement, EmptySubstitution.INSTANCE, PresenterOptions.NameOnly, declaration);
			colorizer.AppendPlainText("' of type '");
			colorizer.AppendExpressionType(highlighting.Type, false, PresenterOptions.NameOnly);
			colorizer.AppendPlainText("' causes a cycle in the struct layout");
		}
		
		public CycleInStructLayoutErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}