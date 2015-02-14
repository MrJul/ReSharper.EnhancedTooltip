using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.UI.RichText;
using JetBrains.Util;
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Wraps an existing <see cref="ParameterInfoCandidate"/>
	/// and override its <see cref="ICandidate.GetSignature"/> method to provide colored parameter info.
	/// </summary>
	public partial class EnhancedParameterInfoCandidate : ICandidate {

		[NotNull] private readonly ParameterInfoCandidate _underlyingCandidate;
		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		[NotNull] private readonly IContextBoundSettingsStore _settings;

		[NotNull]
		public ParameterInfoCandidate UnderlyingCandidate {
			get { return _underlyingCandidate; }
		}

		public RichTextBlock GetDescription() {
			return _underlyingCandidate.GetDescription();
		}

		public void GetParametersInfo(out string[] paramNames, out RichTextBlock[] paramDescriptions, out bool isParamsArray) {
			_underlyingCandidate.GetParametersInfo(out paramNames, out paramDescriptions, out isParamsArray);
		}

		[NotNull]
		private RichText GetSignatureCore(string[] namedArguments, AnnotationsDisplayKind? showAnnotations, out TextRange[] parameterRanges, out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			if (showAnnotations == null)
				showAnnotations = _settings.GetValue((ParameterInfoSettings s) => s.ShowAnnotations);

			// TODO: handle named arguments with reordering; currently falling back to non-colored display
			if (namedArguments.Any(s => s != null)) {
				string signature = _underlyingCandidate.GetSignature(namedArguments, showAnnotations.Value, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
				if (!IsIdentityMap(mapToOriginalOrder))
					return signature;
			}

			var options = PresenterOptions.ForParameterInfo(_settings, showAnnotations.Value);
			bool useReSharperColors = _settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled);
			PresentedInfo presentedInfo;
			InvocationCandidate invocationCandidate = _underlyingCandidate.InvocationCandidate;
			var elementInstance = new DeclaredElementInstance(invocationCandidate.Element, invocationCandidate.Substitution);
			
			RichText richText = _colorizerPresenter.TryPresent(elementInstance, options, _underlyingCandidate.Language, useReSharperColors, out presentedInfo);
			if (richText == null)
				return _underlyingCandidate.GetSignature(namedArguments, showAnnotations.Value, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);

			if (presentedInfo.Parameters.Count == 0) {
				parameterRanges = EmptyArray<TextRange>.Instance;
				mapToOriginalOrder = EmptyArray<int>.Instance;
				extensionMethodInfo = ExtensionMethodInfo.NoExtension;
			}
			else if (presentedInfo.IsExtensionMethod) {
				parameterRanges = presentedInfo.Parameters.Skip(1).ToArray();
				mapToOriginalOrder = CreateIdentityMap(presentedInfo.Parameters.Count - 1);
				TextRange firstParameterRange = presentedInfo.Parameters[0].TrimLeft(5); // keeps "this " highlighted with the keyword color
				extensionMethodInfo = new ExtensionMethodInfo(firstParameterRange, TextRange.InvalidRange);
			}
			else {
				parameterRanges = presentedInfo.Parameters.ToArray();
				mapToOriginalOrder = CreateIdentityMap(presentedInfo.Parameters.Count);
				extensionMethodInfo = ExtensionMethodInfo.NoExtension;
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

		private static bool IsIdentityMap([NotNull] int[] map) {
			int length = map.Length;
			for (int i = 0; i < length; ++i) {
				if (map[i] != i)
					return false;
			}
			return true;
		}

		public bool IsFilteredOut {
			get { return _underlyingCandidate.IsFilteredOut; }
			set { _underlyingCandidate.IsFilteredOut = value; }
		}

		public bool IsObsolete {
			get { return _underlyingCandidate.IsObsolete; }
		}

		public bool Matches(IDeclaredElement signature) {
			return _underlyingCandidate.Matches(signature);
		}

		public RichTextBlock ObsoleteDescription {
			get { return _underlyingCandidate.ObsoleteDescription; }
		}

		public int PositionalParameterCount {
			get { return _underlyingCandidate.PositionalParameterCount; }
		}

		public EnhancedParameterInfoCandidate([NotNull] ParameterInfoCandidate underlyingCandidate, [NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IContextBoundSettingsStore settings) {
			_underlyingCandidate = underlyingCandidate;
			_colorizerPresenter = colorizerPresenter;
			_settings = settings;
		}

	}

}