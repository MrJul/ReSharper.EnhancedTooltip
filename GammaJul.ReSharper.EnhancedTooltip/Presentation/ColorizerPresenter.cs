using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// A component that can create a colored representation of a <see cref="IDeclaredElement"/>.
	/// Currently only uses <see cref="CSharpColorizer"/>.
	/// </summary>
	[SolutionComponent]
	public class ColorizerPresenter {

		private readonly TextStyleHighlighterManager _textStyleHighlighterManager;

		/// <summary>
		/// Presents a given <see cref="DeclaredElementInstance"/> using a colorizer.
		/// </summary>
		/// <param name="declaredElementInstance">The declared element instance.</param>
		/// <param name="options">The options to use to present the element.</param>
		/// <param name="languageType">The type of language used to present the element.</param>
		/// <param name="nameHighlightingAttributeId">The highlighting attribute identifier used to colorize the name.</param>
		/// <returns>A <see cref="RichText"/> representing <paramref name="declaredElementInstance"/>.</returns>
		[CanBeNull]
		public RichText TryPresent([NotNull] DeclaredElementInstance declaredElementInstance, [NotNull] PresenterOptions options,
			[NotNull] PsiLanguageType languageType, [CanBeNull] string nameHighlightingAttributeId) {
			PresentedInfo presentedInfo;
			return TryPresent(declaredElementInstance, options, languageType, nameHighlightingAttributeId, out presentedInfo);
		}

		/// <summary>
		/// Presents a given <see cref="DeclaredElementInstance"/> using a colorizer.
		/// </summary>
		/// <param name="declaredElementInstance">The declared element instance.</param>
		/// <param name="options">The options to use to present the element.</param>
		/// <param name="languageType">The type of language used to present the element.</param>
		/// <param name="nameHighlightingAttributeId">The highlighting attribute identifier used to colorize the name.</param>
		/// <param name="presentedInfo">When the method returns, a <see cref="PresentedInfo"/> containing range information about the presented element.</param>
		/// <returns>A <see cref="RichText"/> representing <paramref name="declaredElementInstance"/>.</returns>
		[CanBeNull]
		public RichText TryPresent([NotNull] DeclaredElementInstance declaredElementInstance, [NotNull] PresenterOptions options,
			[NotNull] PsiLanguageType languageType, [CanBeNull] string nameHighlightingAttributeId, [NotNull] out PresentedInfo presentedInfo) {
			
			var richText = new RichText();
			presentedInfo = new PresentedInfo();
			IColorizer colorizer = TryCreateColorizer(richText, options, languageType, presentedInfo);
			if (colorizer == null)
				return null;

			colorizer.AppendDeclaredElement(declaredElementInstance.Element, declaredElementInstance.Substitution, nameHighlightingAttributeId);
			return richText;
		}

		[CanBeNull]
		private IColorizer TryCreateColorizer([NotNull] RichText richText, [NotNull] PresenterOptions options,
			[NotNull] PsiLanguageType languageType, [NotNull] PresentedInfo presentedInfo) {
			// TODO: remove constructor parameters and resolve as a language service
			if (languageType.Is<CSharpLanguage>())
				return new CSharpColorizer(richText, options, presentedInfo, _textStyleHighlighterManager);
			return null;
		}

		public ColorizerPresenter([NotNull] TextStyleHighlighterManager textStyleHighlighterManager) {
			_textStyleHighlighterManager = textStyleHighlighterManager;
		}

	}

}