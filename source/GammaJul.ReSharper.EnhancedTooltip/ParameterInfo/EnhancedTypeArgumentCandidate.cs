using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Wraps an existing <see cref="TypeArgumentCandidate"/>
	/// and override its <see cref="ICandidate.GetSignature"/> method to provide colored parameter info.
	/// </summary>
	public class EnhancedTypeArgumentCandidate : EnhancedCandidate<TypeArgumentCandidate> {
		
		private readonly ColorizerPresenter _colorizerPresenter;

		protected override PresenterOptions GetPresenterOptions(IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations)
			=> PresenterOptions.ForTypeArgumentInfo(settings, showAnnotations.ToAttributesDisplayKind());

		protected override RichText? TryGetSignatureCore(
			PresenterOptions options,
			HighlighterIdProvider highlighterIdProvider,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			parameterRanges = EmptyArray<TextRange>.Instance;
			mapToOriginalOrder = EmptyArray<int>.Instance;
			extensionMethodInfo = ExtensionMethodInfo.NoExtension;

			ITypeParametersOwner typeElement = UnderlyingCandidate.TypeElement;
			var elementInstance = new DeclaredElementInstance(typeElement, typeElement.IdSubstitution);
			if (_colorizerPresenter.TryPresent(elementInstance, options, UnderlyingCandidate.Language, highlighterIdProvider, null, out PresentedInfo presentedInfo) is not { } richText)
				return null;

			parameterRanges = presentedInfo.TypeParameters.ToArray();
			return richText;
		}

		public EnhancedTypeArgumentCandidate(
			TypeArgumentCandidate underlyingCandidate,
			IContextBoundSettingsStore settings,
			HighlighterIdProviderFactory highlighterIdProviderFactory,
			ColorizerPresenter colorizerPresenter)
			: base(underlyingCandidate, settings, highlighterIdProviderFactory) {
			_colorizerPresenter = colorizerPresenter;
		}

	}

}