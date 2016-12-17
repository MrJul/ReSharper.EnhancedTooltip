using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class ClassIsAbstractAndSealedErrorEnhancer : CSharpHighlightingEnhancer<ClassIsAbstractAndSealedError> {

		protected override void AppendTooltip(ClassIsAbstractAndSealedError highlighting, CSharpColorizer colorizer) {
			ITypeElement declaredElement = highlighting.ClassDeclaration.DeclaredElement;

			colorizer.AppendPlainText("Class '");
			if (declaredElement != null)
				colorizer.AppendDeclaredElement(declaredElement, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedName, highlighting.ClassDeclaration);
			else
				colorizer.AppendClassName(highlighting.ClassDeclaration.DeclaredName);
			colorizer.AppendPlainText("' cannot be both ");
			colorizer.AppendKeyword("abstract");
			colorizer.AppendPlainText(" and ");
			colorizer.AppendKeyword("sealed");
		}
		
		public ClassIsAbstractAndSealedErrorEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}