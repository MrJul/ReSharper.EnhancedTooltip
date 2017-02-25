using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Wraps an existing <see cref="ParameterInfoCandidate"/>
	/// and override its <see cref="ICandidate.GetSignature"/> method to provide colored parameter info.
	/// </summary>
	public class EnhancedParameterInfoCandidate : EnhancedCandidate<ParameterInfoCandidate> {

		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;

		protected override PresenterOptions GetPresenterOptions(IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations)
			=> PresenterOptions.ForParameterInfo(settings, showAnnotations);

		protected override RichText TryGetSignatureCore(
			PresenterOptions options,
			HighlighterIdProvider highlighterIdProvider,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			parameterRanges = EmptyArray<TextRange>.Instance;
			mapToOriginalOrder = EmptyArray<int>.Instance;
			extensionMethodInfo = ExtensionMethodInfo.NoExtension;

			PresentedInfo presentedInfo;
			InvocationCandidate invocationCandidate = UnderlyingCandidate.InvocationCandidate;
			var elementInstance = new DeclaredElementInstance(invocationCandidate.Element, invocationCandidate.Substitution);
			RichText richText = _colorizerPresenter.TryPresent(elementInstance, options, UnderlyingCandidate.Language, highlighterIdProvider, null, out presentedInfo);
			if (richText == null)
				return null;

			if (presentedInfo.Parameters.Count > 0) {
				if (presentedInfo.IsExtensionMethod && UnderlyingCandidate.InvocationCandidate.IsExtensionMethod) {
					parameterRanges = presentedInfo.Parameters.Skip(1).ToArray();
					mapToOriginalOrder = CreateIdentityMap(presentedInfo.Parameters.Count - 1);
					TextRange firstParameterRange = presentedInfo.Parameters[0].TrimLeft(5); // keeps "this " highlighted with the keyword color
					extensionMethodInfo = new ExtensionMethodInfo(firstParameterRange, TextRange.InvalidRange);
				}
				else {
					parameterRanges = presentedInfo.Parameters.ToArray();
					mapToOriginalOrder = CreateIdentityMap(presentedInfo.Parameters.Count);
				}
			}

			return richText;
		}
		
		[NotNull]
		private static int[] CreateIdentityMap(int length) {
			var map = new int[length];
			for (int i = 0; i < length; ++i)
				map[i] = i;
			return map;
		}
		
		public EnhancedParameterInfoCandidate(
			[NotNull] ParameterInfoCandidate underlyingCandidate,
			[NotNull] IContextBoundSettingsStore settings,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory,
			[NotNull] ColorizerPresenter colorizerPresenter)
			: base(underlyingCandidate, settings, highlighterIdProviderFactory) {
			_colorizerPresenter = colorizerPresenter;
		}

	}

}