using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Wraps an existing <see cref="ParameterInfoCandidate"/>
	/// and override its <see cref="ICandidate.GetSignature"/> method to provide colored parameter info.
	/// </summary>
	public class EnhancedParameterInfoCandidate : EnhancedCandidate<ParameterInfoCandidate> {

		private readonly ColorizerPresenter _colorizerPresenter;

		protected override PresenterOptions GetPresenterOptions(IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations)
			=> PresenterOptions.ForParameterInfo(settings, showAnnotations.ToAttributesDisplayKind());
		
		protected override RichText? TryGetSignatureCore(
			PresenterOptions options,
			HighlighterIdProvider highlighterIdProvider,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			parameterRanges = EmptyArray<TextRange>.Instance;
			mapToOriginalOrder = EmptyArray<int>.Instance;
			extensionMethodInfo = ExtensionMethodInfo.NoExtension;

			var (declaredElement, substitution) = UnderlyingCandidate.InvocationCandidate;
			var elementInstance = new DeclaredElementInstance(declaredElement, substitution);
			RichText? richText = _colorizerPresenter.TryPresent(elementInstance, options, UnderlyingCandidate.Language, highlighterIdProvider, null, out PresentedInfo presentedInfo);
			if (richText is null)
				return null;

			if (presentedInfo.Parameters.Count > 0) {
				if (presentedInfo.IsExtensionMethod && UnderlyingCandidate.InvocationCandidate.IsExtensionMethodInvocation) {
					parameterRanges = presentedInfo.Parameters.Skip(1).ToArray();
					mapToOriginalOrder = CreateIdentityMap(presentedInfo.Parameters.Count - 1);
					extensionMethodInfo = new ExtensionMethodInfo(presentedInfo.Parameters[0], TextRange.InvalidRange);
				}
				else {
					parameterRanges = presentedInfo.Parameters.ToArray();
					mapToOriginalOrder = CreateIdentityMap(presentedInfo.Parameters.Count);
				}
			}

			return richText;
		}
		
		private static int[] CreateIdentityMap(int length) {
			var map = new int[length];
			for (int i = 0; i < length; ++i)
				map[i] = i;
			return map;
		}
		
		public EnhancedParameterInfoCandidate(
			ParameterInfoCandidate underlyingCandidate,
			IContextBoundSettingsStore settings,
			HighlighterIdProviderFactory highlighterIdProviderFactory,
			ColorizerPresenter colorizerPresenter)
			: base(underlyingCandidate, settings, highlighterIdProviderFactory) {
			_colorizerPresenter = colorizerPresenter;
		}

	}

}