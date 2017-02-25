using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
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
	[SolutionComponent]
	public class ColorizerPresenter {

		[NotNull] private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		[NotNull] private readonly CodeAnnotationsConfiguration _codeAnnotationsConfiguration;

		/// <summary>
		/// Presents a given <see cref="DeclaredElementInstance"/> using a colorizer.
		/// </summary>
		/// <param name="declaredElementInstance">The declared element instance.</param>
		/// <param name="options">The options to use to present the element.</param>
		/// <param name="languageType">The type of language used to present the element.</param>
		/// <param name="highlighterIdProvider">An object determining which highlightings to use.</param>
		/// <param name="contextualNode">The tree node where the element is presented.</param>
		/// <returns>A <see cref="RichText"/> representing <paramref name="declaredElementInstance"/>.</returns>
		[CanBeNull]
		public RichText TryPresent(
			[NotNull] DeclaredElementInstance declaredElementInstance,
			[NotNull] PresenterOptions options,
			[NotNull] PsiLanguageType languageType,
			[NotNull] HighlighterIdProvider highlighterIdProvider,
			[CanBeNull] ITreeNode contextualNode) {

			PresentedInfo presentedInfo;
			return TryPresent(declaredElementInstance, options, languageType, highlighterIdProvider, contextualNode, out presentedInfo);
		}

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
		[CanBeNull]
		public RichText TryPresent(
			[NotNull] DeclaredElementInstance declaredElementInstance,
			[NotNull] PresenterOptions options,
			[NotNull] PsiLanguageType languageType,
			[NotNull] HighlighterIdProvider highlighterIdProvider,
			[CanBeNull] ITreeNode contextualNode,
			[NotNull] out PresentedInfo presentedInfo) {

			var richText = new RichText();
			IColorizer colorizer = TryCreateColorizer(richText, languageType, highlighterIdProvider);
			if (colorizer == null) {
				presentedInfo = new PresentedInfo();
				return null;
			}

			presentedInfo = colorizer.AppendDeclaredElement(declaredElementInstance.Element, declaredElementInstance.Substitution, options, contextualNode);
			return richText;
		}

		[CanBeNull]
		public RichText TryPresent(
			[NotNull] ILiteralExpression literalExpression,
			[NotNull] PresenterOptions options,
			[NotNull] PsiLanguageType languageType,
			[NotNull] HighlighterIdProvider highlighterIdProvider) {

			var richText = new RichText();
			IColorizer colorizer = TryCreateColorizer(richText, languageType, highlighterIdProvider);
			if (colorizer == null)
				return null;

			colorizer.AppendLiteralExpression(literalExpression, options);
			return richText;
		}

		[CanBeNull]
		private IColorizer TryCreateColorizer([NotNull] RichText richText, [NotNull] PsiLanguageType languageType, [NotNull] HighlighterIdProvider highlighterIdProvider) {
			// TODO: add a language service instead of checking the language
			if (languageType.Is<CSharpLanguage>())
				return new CSharpColorizer(richText, _textStyleHighlighterManager, _codeAnnotationsConfiguration, highlighterIdProvider);
			return null;
		}

		public ColorizerPresenter(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration) {
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsConfiguration = codeAnnotationsConfiguration;
		}

	}

}