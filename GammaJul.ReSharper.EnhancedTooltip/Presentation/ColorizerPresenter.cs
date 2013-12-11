using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
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
		/// <param name="nameHighlightingAttributeId">The highlighting attribute identifier used to colorize the name.</param>
		/// <returns>A <see cref="RichText"/> representing <paramref name="declaredElementInstance"/>.</returns>
		[NotNull]
		public RichText Present([NotNull] DeclaredElementInstance declaredElementInstance, [NotNull] PresenterOptions options,
			[CanBeNull] string nameHighlightingAttributeId) {
			PresentedInfo presentedInfo;
			return Present(declaredElementInstance, options, nameHighlightingAttributeId, out presentedInfo);
		}

		/// <summary>
		/// Presents a given <see cref="DeclaredElementInstance"/> using a colorizer.
		/// </summary>
		/// <param name="declaredElementInstance">The declared element instance.</param>
		/// <param name="options">The options to use to present the element.</param>
		/// <param name="nameHighlightingAttributeId">The highlighting attribute identifier used to colorize the name.</param>
		/// <param name="presentedInfo">When the method returns, a <see cref="PresentedInfo"/> containing range information about the presented element.</param>
		/// <returns>A <see cref="RichText"/> representing <paramref name="declaredElementInstance"/>.</returns>
		[NotNull]
		public RichText Present([NotNull] DeclaredElementInstance declaredElementInstance, [NotNull] PresenterOptions options,
			[CanBeNull] string nameHighlightingAttributeId, [NotNull] out PresentedInfo presentedInfo) {
			
			var richText = new RichText();
			presentedInfo = new PresentedInfo();
			IColorizer colorizer = CreateColorizer(richText, options, presentedInfo);
			colorizer.AppendDeclaredElement(declaredElementInstance.Element, declaredElementInstance.Substitution, nameHighlightingAttributeId);
			return richText;
		}

		[NotNull]
		private IColorizer CreateColorizer([NotNull] RichText richText, [NotNull] PresenterOptions options, [NotNull] PresentedInfo presentedInfo) {
			// TODO: handle other languages
			return new CSharpColorizer(richText, options, presentedInfo, _textStyleHighlighterManager);
		}

		public ColorizerPresenter([NotNull] TextStyleHighlighterManager textStyleHighlighterManager) {
			_textStyleHighlighterManager = textStyleHighlighterManager;
		}

	}

}