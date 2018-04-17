using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresenterOptions {

		public AttributesFormattingMode AttributesFormattingMode { get; private set; }

		public ExternalCodeNamespaceDisplayKind ExternalCodeNamespaceDisplayKind { get; private set; }

		public bool FormatDelegatesAsLambdas { get; private set; }

		public ParametersFormattingMode ParametersFormattingMode { get; private set; }

		public bool ShowAccessors { get; private set; }

		public bool ShowAccessRights { get; private set; }

		public bool ShowConstantValue { get; private set; }

		public bool ShowDefaultValues { get; private set; }

		public AttributesDisplayKind ShowElementAttributes { get; private set; }

		public bool ShowElementAttributesArguments { get; private set; }

		public ElementKindDisplay ShowElementKind { get; private set; }

		public ElementTypeDisplay ShowElementType { get; private set; }

		public bool ShowEmptyParametersText { get; private set; }

		public bool ShowExplicitInterface { get; private set; }

		public bool ShowModifiers { get; private set; }

		public bool ShowName { get; private set; }

		public AttributesDisplayKind ShowParametersAttributes { get; private set; }

		public bool ShowParametersAttributesArguments { get; private set; }

		public bool ShowParametersName { get; private set; }
		
		public bool ShowParametersType { get; private set; }

		public QualifierDisplays ShowQualifiers { get; private set; }

		public bool ShowTypeParameters { get; private set; }

		public bool ShowTypeParametersVariance { get; private set; }

		public SolutionCodeNamespaceDisplayKind SolutionCodeNamespaceDisplayKind { get; private set; }

		public bool UseAttributeClassKind { get; private set; }

		public bool UseClassModifiersInKind { get; private set; }

		public bool UseExtensionMethodKind { get; private set; }

		public bool UseMethodModifiersInKind { get; private set; }

		public bool UseShortNullableForm { get; private set; }

		public bool UseStructModifiersInKind { get; private set; }

		public bool UseTypeKeywords { get; private set; }

		[NotNull]
		public static PresenterOptions ForIdentifierToolTip([NotNull] IContextBoundSettingsStore settings, bool showElementType)
			=> new PresenterOptions {
				AttributesFormattingMode = settings.GetValue((IdentifierTooltipSettings s) => s.AttributesFormattingMode),
				ExternalCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.ExternalCodeNamespaceDisplayKind),
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey s) => s.DelegatesAsLambdas),
				ParametersFormattingMode = settings.GetValue((IdentifierTooltipSettings s) => s.ParametersFormattingMode),
				ShowAccessors = settings.GetValue((IdentifierTooltipSettings s) => s.ShowAccessors),
				ShowAccessRights = settings.GetValue((IdentifierTooltipSettings s) => s.ShowAccessRights),
				ShowConstantValue = true,
				ShowDefaultValues = true,
				ShowElementAttributes = settings.GetValue((IdentifierTooltipSettings s) => s.ShowIdentifierAttributes),
				ShowElementAttributesArguments = settings.GetValue((IdentifierTooltipSettings s) => s.ShowIdentifierAttributesArguments),
				ShowElementKind = settings.GetValue((IdentifierTooltipSettings s) => s.ShowKind) ? ElementKindDisplay.Stylized : ElementKindDisplay.None,
				ShowElementType = showElementType ? ElementTypeDisplay.Before : ElementTypeDisplay.None,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersAttributes = settings.GetValue((IdentifierTooltipSettings s) => s.ShowParametersAttributes),
				ShowParametersAttributesArguments = settings.GetValue((IdentifierTooltipSettings s) => s.ShowParametersAttributesArguments),
				ShowParametersName = true,
				ShowParametersType = true,
				ShowTypeParameters = true,
				ShowTypeParametersVariance = false,
				ShowQualifiers = QualifierDisplays.Member,
				SolutionCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.SolutionCodeNamespaceDisplayKind),
				UseAttributeClassKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseAttributeClassKind),
				UseClassModifiersInKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseClassModifiersInKind),
				UseExtensionMethodKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseExtensionMethodKind),
				UseMethodModifiersInKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseMethodModifiersInKind),
				UseShortNullableForm = settings.GetValue((IdentifierTooltipSettings s) => s.UseShortNullableForm),
				UseStructModifiersInKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseStructModifiersInKind),
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForArgumentRoleParametersOwnerToolTip([NotNull] IContextBoundSettingsStore settings)
			=> new PresenterOptions {
				AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
				FormatDelegatesAsLambdas = false,
				ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = false,
				ShowElementAttributes = AttributesDisplayKind.Never,
				ShowElementAttributesArguments = false,
				ShowElementKind = ElementKindDisplay.Standard,
				ShowElementType = ElementTypeDisplay.None,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersAttributes = AttributesDisplayKind.Never,
				ShowParametersAttributesArguments = false,
				ShowParametersName = false,
				ShowParametersType = false,
				ShowQualifiers = QualifierDisplays.None,
				ShowTypeParameters = true,
				ShowTypeParametersVariance = false,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseShortNullableForm = settings.GetValue((IdentifierTooltipSettings s) => s.UseShortNullableForm),
				UseStructModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForArgumentRoleParameterToolTip([NotNull] IContextBoundSettingsStore settings)
			=> new PresenterOptions {
				AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
				FormatDelegatesAsLambdas = false,
				ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = false,
				ShowElementAttributes = AttributesDisplayKind.Never,
				ShowElementAttributesArguments = false,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersAttributes = AttributesDisplayKind.Never,
				ShowParametersAttributesArguments = false,
				ShowParametersName = false,
				ShowParametersType = false,
				ShowQualifiers = QualifierDisplays.None,
				ShowTypeParameters = false,
				ShowTypeParametersVariance = false,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseShortNullableForm = settings.GetValue((IdentifierTooltipSettings s) => s.UseShortNullableForm),
				UseStructModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForParameterInfo([NotNull] IContextBoundSettingsStore settings, AttributesDisplayKind attributesDisplayKind)
			=> new PresenterOptions {
				AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = true,
				ShowElementAttributes = AttributesDisplayKind.Never,
				ShowElementAttributesArguments = false,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementType = ElementTypeDisplay.After,
				ShowEmptyParametersText = settings.GetValue((ParameterInfoSettings s) => s.ShowEmptyParametersText),
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = false,
				ShowParametersAttributes = attributesDisplayKind,
				ShowParametersAttributesArguments = false,
				ShowParametersName = true,
				ShowParametersType = true,
				ShowQualifiers = QualifierDisplays.None,
				ShowTypeParameters = false,
				ShowTypeParametersVariance = false,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseShortNullableForm = true,
				UseStructModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((ParameterInfoSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForTypeArgumentInfo([NotNull] IContextBoundSettingsStore settings, AttributesDisplayKind attributesDisplayKind)
			=> new PresenterOptions {
				AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
				FormatDelegatesAsLambdas = false,
				ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = false,
				ShowElementAttributes = AttributesDisplayKind.Never,
				ShowElementAttributesArguments = false,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementType = ElementTypeDisplay.None,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersAttributes = attributesDisplayKind,
				ShowParametersAttributesArguments = false,
				ShowParametersName = false,
				ShowParametersType = false,
				ShowQualifiers = QualifierDisplays.Member,
				ShowTypeParameters = true,
				ShowTypeParametersVariance = true,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseShortNullableForm = true,
				UseStructModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((ParameterInfoSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForBaseTypeOrImplementedInterfaceTooltip([NotNull] IContextBoundSettingsStore settings)
			=> new PresenterOptions {
				AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
				ExternalCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.ExternalCodeNamespaceDisplayKind),
				FormatDelegatesAsLambdas = false,
				ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = true,
				ShowElementAttributes = AttributesDisplayKind.Never,
				ShowElementAttributesArguments = false,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersAttributes = AttributesDisplayKind.Never,
				ShowParametersAttributesArguments = false,
				ShowParametersName = false,
				ShowParametersType = true,
				ShowQualifiers = QualifierDisplays.Member,
				ShowTypeParameters = true,
				ShowTypeParametersVariance = false,
				SolutionCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.SolutionCodeNamespaceDisplayKind),
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseShortNullableForm = true,
				UseStructModifiersInKind = false,
				UseTypeKeywords = true
			};

		[NotNull]
		public static readonly PresenterOptions FullWithoutParameterNames = new PresenterOptions {
			AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
			FormatDelegatesAsLambdas = false,
			ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementAttributes = AttributesDisplayKind.Never,
			ShowElementAttributesArguments = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = true,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersAttributes = AttributesDisplayKind.Never,
			ShowParametersAttributesArguments = false,
			ShowParametersName = false,
			ShowParametersType = true,
			ShowTypeParametersVariance = false,
			ShowQualifiers = QualifierDisplays.Everywhere,
			ShowTypeParameters = true,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseShortNullableForm = true,
			UseStructModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedMember = new PresenterOptions {
			AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
			FormatDelegatesAsLambdas = false,
			ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementAttributes = AttributesDisplayKind.Never,
			ShowElementAttributesArguments = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersAttributes = AttributesDisplayKind.Never,
			ShowParametersAttributesArguments = false,
			ShowParametersName = false,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.Member,
			ShowTypeParameters = true,
			ShowTypeParametersVariance = false,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseShortNullableForm = true,
			UseStructModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedName = new PresenterOptions {
			AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
			FormatDelegatesAsLambdas = false,
			ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementAttributes = AttributesDisplayKind.Never,
			ShowElementAttributesArguments = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersAttributes = AttributesDisplayKind.Never,
			ShowParametersAttributesArguments = false,
			ShowParametersName = false,
			ShowParametersType = false,
			ShowQualifiers = QualifierDisplays.Member,
			ShowTypeParameters = true,
			ShowTypeParametersVariance = false,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseShortNullableForm = true,
			UseStructModifiersInKind = false,
			UseTypeKeywords = true
		};
		
		[NotNull]
		public static readonly PresenterOptions ForInterfaceMember = new PresenterOptions {
			AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
			FormatDelegatesAsLambdas = false,
			ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementAttributes = AttributesDisplayKind.Never,
			ShowElementAttributesArguments = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = true,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersAttributes = AttributesDisplayKind.Never,
			ShowParametersAttributesArguments = false,
			ShowParametersName = false,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.None,
			ShowTypeParameters = true,
			ShowTypeParametersVariance = false,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseShortNullableForm = true,
			UseStructModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions NameOnly = new PresenterOptions {
			AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
			FormatDelegatesAsLambdas = false,
			ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementAttributes = AttributesDisplayKind.Never,
			ShowElementAttributesArguments = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersAttributes = AttributesDisplayKind.Never,
			ShowParametersAttributesArguments = false,
			ShowParametersName = false,
			ShowParametersType = false,
			ShowQualifiers = QualifierDisplays.None,
			ShowTypeParameters = false,
			ShowTypeParametersVariance = false,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseShortNullableForm = true,
			UseStructModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions ParameterTypesOnly = new PresenterOptions {
			AttributesFormattingMode = AttributesFormattingMode.AllOnCurrentLine,
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
			FormatDelegatesAsLambdas = false,
			ParametersFormattingMode = ParametersFormattingMode.AllOnCurrentLine,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementAttributes = AttributesDisplayKind.Never,
			ShowElementAttributesArguments = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = false,
			ShowParametersAttributes = AttributesDisplayKind.Never,
			ShowParametersAttributesArguments = false,
			ShowParametersName = false,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.None,
			ShowTypeParameters = false,
			ShowTypeParametersVariance = false,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseShortNullableForm = true,
			UseStructModifiersInKind = false,
			UseTypeKeywords = true
		};

		private PresenterOptions() {
		}

	}

}