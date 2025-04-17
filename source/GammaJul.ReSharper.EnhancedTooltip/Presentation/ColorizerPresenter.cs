using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// A component that can create a colored representation of a <see cref="IDeclaredElement"/>.
	/// Currently only uses <see cref="CSharpColorizer"/>.
	/// </summary>
	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	public class ColorizerPresenter {

		private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		private readonly CodeAnnotationsConfiguration _codeAnnotationsConfiguration;
		
		/// <summary>
		/// Presents a given <see cref="DeclaredElementInstance"/> using a colorizer.
		/// </summary>
		/// <param name="declaredElementInstance">The declared element instance.</param>
		/// <param name="options">The options to use to present the element.</param>
		/// <param name="languageType">The type of language used to present the element.</param>
		/// <param name="highlighterIdProvider">An object determining which highlightings to use.</param>
		/// <param name="contextualNode">The tree node where the element is presented.</param>
		/// <param name="presentedInfo">When the method returns, a <see cref="PresentedInfo"/> containing range information about the presented element.</param>
		/// <returns>A <see cref="RichText"/> representing <paramref name="declaredElementInstance"/>.</returns>
		public RichText? TryPresent(
			DeclaredElementInstance declaredElementInstance,
			PresenterOptions options,
			PsiLanguageType languageType,
			HighlighterIdProvider highlighterIdProvider,
			ITreeNode? contextualNode,
			out PresentedInfo presentedInfo) {

			var richText = new RichText();
			if (TryCreateColorizer(richText, languageType, highlighterIdProvider) is { } colorizer) {
				presentedInfo = colorizer.AppendDeclaredElement(declaredElementInstance.Element, declaredElementInstance.Substitution, options, contextualNode);
				return richText;
			}

			presentedInfo = new PresentedInfo();
			return null;
		}

		public RichText? TryPresent(
			ITreeNode presentableNode,
			PresenterOptions options,
			PsiLanguageType languageType,
			HighlighterIdProvider highlighterIdProvider) {

			var richText = new RichText();
			if (TryCreateColorizer(richText, languageType, highlighterIdProvider) is { } colorizer) {
				colorizer.AppendPresentableNode(presentableNode, options);
				return richText;
			}

			return null;
		}

		private IColorizer? TryCreateColorizer(RichText richText, PsiLanguageType languageType, HighlighterIdProvider highlighterIdProvider) {
			// TODO: add a language service instead of checking the language
			if (languageType.Is<CSharpLanguage>())
				return new CSharpColorizer(richText, _textStyleHighlighterManager, _codeAnnotationsConfiguration, highlighterIdProvider);
			return null;
		}

		public ColorizerPresenter(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration) {
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsConfiguration = codeAnnotationsConfiguration;
		}

	}

}