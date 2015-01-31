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

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Wraps an existing <see cref="ParameterInfoCandidate"/>
	/// and override its <see cref="ICandidate.GetSignature"/> method to provide colored parameter info.
	/// </summary>
	public partial class EnhancedParameterInfoCandidate : ICandidate {

		[NotNull] private readonly ParameterInfoCandidate _candidate;
		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		[NotNull] private readonly IContextBoundSettingsStore _settings;

		public RichTextBlock GetDescription() {
			return _candidate.GetDescription();
		}

		public void GetParametersInfo(out string[] paramNames, out RichTextBlock[] paramDescriptions, out bool isParamsArray) {
			_candidate.GetParametersInfo(out paramNames, out paramDescriptions, out isParamsArray);
		}

		[NotNull]
		private RichText GetSignatureCore(string[] namedArguments, AnnotationsDisplayKind? showAnnotations, out TextRange[] parameterRanges, out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			if (showAnnotations == null)
				showAnnotations = _settings.GetValue((ParameterInfoSettings s) => s.ShowAnnotations);

			// TODO: handle named arguments with reordering; currently falling back to non-colored display
			if (namedArguments.Any(s => s != null)) {
				string signature = _candidate.GetSignature(namedArguments, showAnnotations.Value, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
				if (!IsIdentityMap(mapToOriginalOrder))
					return signature;
			}

			var options = PresenterOptions.ForParameterInfo(_settings, showAnnotations.Value);
			PresentedInfo presentedInfo;
			InvocationCandidate invocationCandidate = _candidate.InvocationCandidate;
			var elementInstance = new DeclaredElementInstance(invocationCandidate.Element, invocationCandidate.Substitution);
			
			RichText richText = _colorizerPresenter.TryPresent(elementInstance, options, _candidate.Language, null, out presentedInfo);
			if (richText == null)
				return _candidate.GetSignature(namedArguments, showAnnotations.Value, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);

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
			get { return _candidate.IsFilteredOut; }
			set { _candidate.IsFilteredOut = value; }
		}

		public bool IsObsolete {
			get { return _candidate.IsObsolete; }
		}

		public bool Matches(IDeclaredElement signature) {
			return _candidate.Matches(signature);
		}

		public RichTextBlock ObsoleteDescription {
			get { return _candidate.ObsoleteDescription; }
		}

		public int PositionalParameterCount {
			get { return _candidate.PositionalParameterCount; }
		}

		public EnhancedParameterInfoCandidate([NotNull] ParameterInfoCandidate candidate, [NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IContextBoundSettingsStore settings) {
			_candidate = candidate;
			_colorizerPresenter = colorizerPresenter;
			_settings = settings;
		}

	}

}