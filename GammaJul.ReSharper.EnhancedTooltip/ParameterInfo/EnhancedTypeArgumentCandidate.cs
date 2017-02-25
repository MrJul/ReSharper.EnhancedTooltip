using System.Reflection;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
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

		// TODO: remove reflection if/when TypeArgumentCandidate.myTypeElement/myLanguage are public exposed
		[CanBeNull] private static readonly FieldInfo _typeElementField = FindField("myTypeElement");
		[CanBeNull] private static readonly FieldInfo _languageField = FindField("myLanguage");

		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		[CanBeNull] private readonly ITypeParametersOwner _typeElement;
		[CanBeNull] private readonly PsiLanguageType _language;

		[CanBeNull]
		private static FieldInfo FindField([NotNull] string fieldName)
			=> typeof(TypeArgumentCandidate).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

		protected override PresenterOptions GetPresenterOptions(IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations)
			=> PresenterOptions.ForTypeArgumentInfo(settings, showAnnotations);

		protected override RichText TryGetSignatureCore(
			PresenterOptions options,
			HighlighterIdProvider highlighterIdProvider,
			out TextRange[] parameterRanges,
			out int[] mapToOriginalOrder,
			out ExtensionMethodInfo extensionMethodInfo) {

			parameterRanges = EmptyArray<TextRange>.Instance;
			mapToOriginalOrder = EmptyArray<int>.Instance;
			extensionMethodInfo = ExtensionMethodInfo.NoExtension;
			
			if (_typeElement == null || _language == null)
				return null;

			PresentedInfo presentedInfo;
			var elementInstance = new DeclaredElementInstance(_typeElement, _typeElement.IdSubstitution);
			RichText richText = _colorizerPresenter.TryPresent(elementInstance, options, _language, highlighterIdProvider, null, out presentedInfo);
			if (richText == null)
				return null;

			parameterRanges = presentedInfo.TypeParameters.ToArray();
			return richText;
		}

		public EnhancedTypeArgumentCandidate(
			[NotNull] TypeArgumentCandidate underlyingCandidate,
			[NotNull] IContextBoundSettingsStore settings,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory,
			[NotNull] ColorizerPresenter colorizerPresenter)
			: base(underlyingCandidate, settings, highlighterIdProviderFactory) {
			_colorizerPresenter = colorizerPresenter;
			_typeElement = _typeElementField?.GetValue(underlyingCandidate) as ITypeParametersOwner;
			_language = _languageField?.GetValue(underlyingCandidate) as PsiLanguageType;
		}

	}

}