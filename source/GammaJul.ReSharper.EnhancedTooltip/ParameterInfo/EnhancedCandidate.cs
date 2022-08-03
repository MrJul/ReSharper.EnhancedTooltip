using System;
using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {
	
	public abstract class EnhancedCandidate<TCandidate> : ICandidate
	where TCandidate : class, ICandidate {

		private readonly IContextBoundSettingsStore _settings;
		private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;

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

		public void GetParametersInfo(out ParamPresentationInfo[] paramInfos, out Int32 paramArrayIndex)
			=> UnderlyingCandidate.GetParametersInfo(out paramInfos, out paramArrayIndex);
		
		public override bool Equals(object obj)
			=> obj is EnhancedCandidate<TCandidate> candidate
			&& UnderlyingCandidate.Equals(candidate.UnderlyingCandidate);

		public override int GetHashCode()
			=> UnderlyingCandidate.GetHashCode();

		public RichText GetSignature(
			string[] namedArguments,
			AnnotationsDisplayKind showAnnotations,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			// TODO: handle named arguments with reordering; currently falling back to non-colored display
			if (namedArguments.Any(s => s is not null)) {
				RichText signature = UnderlyingCandidate.GetSignature(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
				if (!IsIdentityMap(mapToOriginalOrder))
					return signature;
			}

			PresenterOptions options = GetPresenterOptions(_settings, showAnnotations);
			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(_settings);

			return TryGetSignatureCore(options, highlighterIdProvider, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo) 
				?? UnderlyingCandidate.GetSignature(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
		}

		protected abstract PresenterOptions GetPresenterOptions(IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations);

		private static bool IsIdentityMap(int[] map) {
			int length = map.Length;
			for (int i = 0; i < length; ++i) {
				if (map[i] != i)
					return false;
			}
			return true;
		}

		protected abstract RichText? TryGetSignatureCore(
			PresenterOptions options,
			HighlighterIdProvider highlighterIdProvider,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo);

		protected EnhancedCandidate(
			TCandidate underlyingCandidate,
			IContextBoundSettingsStore settings,
			HighlighterIdProviderFactory highlighterIdProviderFactory) {
			UnderlyingCandidate = underlyingCandidate;
			_settings = settings;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}