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
	public class EnhancedParameterInfoCandidate : ICandidate {

		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		[NotNull] private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;
		[NotNull] private readonly IContextBoundSettingsStore _settings;

		[NotNull]
		public ParameterInfoCandidate UnderlyingCandidate { get; }

		public RichTextBlock GetDescription()
			=> UnderlyingCandidate.GetDescription();

		public bool IsFilteredOut {
			get { return UnderlyingCandidate.IsFilteredOut; }
			set { UnderlyingCandidate.IsFilteredOut = value; }
		}

		public bool IsObsolete
			=> UnderlyingCandidate.IsObsolete;

		public bool Matches(IDeclaredElement signature)
			=> UnderlyingCandidate.Matches(signature);

		public RichTextBlock ObsoleteDescription
			=> UnderlyingCandidate.ObsoleteDescription;

		public int PositionalParameterCount
			=> UnderlyingCandidate.PositionalParameterCount;

		public void GetParametersInfo(out ParamPresentationInfo[] paramInfos, out bool isParamsArray)
			=> UnderlyingCandidate.GetParametersInfo(out paramInfos, out isParamsArray);

		[NotNull]
		public RichText GetSignature(
			string[] namedArguments,
			AnnotationsDisplayKind showAnnotations,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {
			
			// TODO: handle named arguments with reordering; currently falling back to non-colored display
			if (namedArguments.Any(s => s != null)) {
				string signature = UnderlyingCandidate.GetSignature(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
				if (!IsIdentityMap(mapToOriginalOrder))
					return signature;
			}

			var options = PresenterOptions.ForParameterInfo(_settings, showAnnotations);
			var highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(_settings);
			PresentedInfo presentedInfo;
			InvocationCandidate invocationCandidate = UnderlyingCandidate.InvocationCandidate;
			var elementInstance = new DeclaredElementInstance(invocationCandidate.Element, invocationCandidate.Substitution);
			
			RichText richText = _colorizerPresenter.TryPresent(elementInstance, options, UnderlyingCandidate.Language, highlighterIdProvider, null, out presentedInfo);
			if (richText == null)
				return UnderlyingCandidate.GetSignature(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);

			if (presentedInfo.Parameters.Count == 0) {
				parameterRanges = EmptyArray<TextRange>.Instance;
				mapToOriginalOrder = EmptyArray<int>.Instance;
				extensionMethodInfo = ExtensionMethodInfo.NoExtension;
			}
			else if (presentedInfo.IsExtensionMethod && UnderlyingCandidate.InvocationCandidate.IsExtensionMethod) {
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

		public override bool Equals(object obj) {
			var candidate = obj as EnhancedParameterInfoCandidate;
			return candidate != null && UnderlyingCandidate.Equals(candidate.UnderlyingCandidate);
		}

		public override int GetHashCode()
			=> UnderlyingCandidate.GetHashCode();
		
		public EnhancedParameterInfoCandidate(
			[NotNull] ParameterInfoCandidate underlyingCandidate,
			[NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IContextBoundSettingsStore settings,
			HighlighterIdProviderFactory highlighterIdProviderFactory) {
			UnderlyingCandidate = underlyingCandidate;
			_colorizerPresenter = colorizerPresenter;
			_settings = settings;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}