using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {
	
	public abstract class EnhancedCandidate<TCandidate> : ICandidate
	where TCandidate : class, ICandidate {

		[NotNull] private readonly IContextBoundSettingsStore _settings;
		[NotNull] private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;

		[NotNull]
		public TCandidate UnderlyingCandidate { get; }

		public bool IsFilteredOut {
			get => UnderlyingCandidate.IsFilteredOut;
			set => UnderlyingCandidate.IsFilteredOut = value;
		}

		public bool IsObsolete
			=> UnderlyingCandidate.IsObsolete;

		public RichTextBlock ObsoleteDescription
			=> UnderlyingCandidate.ObsoleteDescription;

		public int PositionalParameterCount
			=> UnderlyingCandidate.PositionalParameterCount;

		public RichTextBlock GetDescription()
			=> UnderlyingCandidate.GetDescription();

		public bool Matches(IDeclaredElement signature)
			=> UnderlyingCandidate.Matches(signature);

		public void GetParametersInfo(out ParamPresentationInfo[] paramInfos, out bool isParamsArray)
			=> UnderlyingCandidate.GetParametersInfo(out paramInfos, out isParamsArray);
		
		public override bool Equals(object obj)
			=> obj is EnhancedCandidate<TCandidate> candidate
			&& UnderlyingCandidate.Equals(candidate.UnderlyingCandidate);

		public override int GetHashCode()
			=> UnderlyingCandidate.GetHashCode();

		[NotNull]
		public RichText GetSignature(
			string[] namedArguments,
			AnnotationsDisplayKind showAnnotations,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			// TODO: handle named arguments with reordering; currently falling back to non-colored display
			if (namedArguments.Any(s => s != null)) {
				RichText signature = UnderlyingCandidate.GetSignature(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
				if (!IsIdentityMap(mapToOriginalOrder))
					return signature;
			}

			PresenterOptions options = GetPresenterOptions(_settings, showAnnotations);
			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(_settings);

			RichText richText = TryGetSignatureCore(options, highlighterIdProvider, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
			if (richText == null)
				return UnderlyingCandidate.GetSignature(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);

			return richText;
		}

		[NotNull]
		protected abstract PresenterOptions GetPresenterOptions([NotNull] IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations);

		private static bool IsIdentityMap([NotNull] int[] map) {
			int length = map.Length;
			for (int i = 0; i < length; ++i) {
				if (map[i] != i)
					return false;
			}
			return true;
		}

		[CanBeNull]
		protected abstract RichText TryGetSignatureCore(
			[NotNull] PresenterOptions options,
			[NotNull] HighlighterIdProvider highlighterIdProvider,
			[NotNull] out TextRange[] parameterRanges,
			[NotNull] out int[] mapToOriginalOrder,
			[NotNull] out ExtensionMethodInfo extensionMethodInfo);

		protected EnhancedCandidate(
			[NotNull] TCandidate underlyingCandidate,
			[NotNull] IContextBoundSettingsStore settings,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory) {
			UnderlyingCandidate = underlyingCandidate;
			_settings = settings;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}