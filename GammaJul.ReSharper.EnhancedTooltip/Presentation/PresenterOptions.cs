using JetBrains.ReSharper.Feature.Services.Lookup;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresenterOptions {

		public ExternalCodeNamespaceDisplayKind ExternalCodeNamespaceDisplayKind { get; private set; }

		public bool FormatDelegatesAsLambdas { get; private set; }

		public bool ShowAccessors { get; private set; }

		public bool ShowAccessRights { get; private set; }

		public bool ShowConstantValue { get; private set; }

		public bool ShowDefaultValues { get; private set; }

		public ElementKindDisplay ShowElementKind { get; private set; }

		public AnnotationsDisplayKind ShowElementAnnotations { get; private set; }

		public ElementTypeDisplay ShowElementType { get; private set; }

		public bool ShowEmptyParametersText { get; private set; }

		public bool ShowExplicitInterface { get; private set; }

		public bool ShowModifiers { get; private set; }

		public bool ShowName { get; private set; }

		public bool ShowParametersName { get; private set; }

		public AnnotationsDisplayKind ShowParametersAnnotations { get; private set; }

		public bool ShowParametersType { get; private set; }

		public QualifierDisplays ShowQualifiers { get; private set; }

		public SolutionCodeNamespaceDisplayKind SolutionCodeNamespaceDisplayKind { get; private set; }

		public bool UseAttributeClassKind { get; private set; }

		public bool UseClassModifiersInKind { get; private set; }

		public bool UseExtensionMethodKind { get; private set; }

		public bool UseMethodModifiersInKind { get; private set; }

		public bool UseTypeKeywords { get; private set; }

		[NotNull]
		public static PresenterOptions ForIdentifierToolTip([NotNull] IContextBoundSettingsStore settings, bool showElementType)
			=> new PresenterOptions {
				ExternalCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.ExternalCodeNamespaceDisplayKind),
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey s) => s.DelegatesAsLambdas),
				ShowAccessors = settings.GetValue((IdentifierTooltipSettings s) => s.ShowAccessors),
				ShowAccessRights = settings.GetValue((IdentifierTooltipSettings s) => s.ShowAccessRights),
				ShowConstantValue = true,
				ShowDefaultValues = true,
				ShowElementKind = settings.GetValue((IdentifierTooltipSettings s) => s.ShowKind) ? ElementKindDisplay.Stylized : ElementKindDisplay.None,
				ShowElementAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations),
				ShowElementType = showElementType ? ElementTypeDisplay.Before : ElementTypeDisplay.None,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersName = true,
				ShowParametersAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowParametersAnnotations),
				ShowParametersType = true,
				ShowQualifiers = QualifierDisplays.Member,
				SolutionCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.SolutionCodeNamespaceDisplayKind),
				UseAttributeClassKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseAttributeClassKind),
				UseClassModifiersInKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseClassModifiersInKind),
				UseExtensionMethodKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseExtensionMethodKind),
				UseMethodModifiersInKind = settings.GetValue((IdentifierTooltipSettings s) => s.UseMethodModifiersInKind),
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForArgumentRoleParametersOwnerToolTip([NotNull] IContextBoundSettingsStore settings)
			=> new PresenterOptions {
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
				FormatDelegatesAsLambdas = false,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = false,
				ShowElementKind = ElementKindDisplay.Standard,
				ShowElementAnnotations = AnnotationsDisplayKind.None,
				ShowElementType = ElementTypeDisplay.None,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersName = false,
				ShowParametersAnnotations = AnnotationsDisplayKind.None,
				ShowParametersType = false,
				ShowQualifiers = QualifierDisplays.None,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForArgumentRoleParameterToolTip([NotNull] IContextBoundSettingsStore settings)
			=> new PresenterOptions {
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
				FormatDelegatesAsLambdas = false,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = false,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementAnnotations = AnnotationsDisplayKind.None,
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersName = false,
				ShowParametersAnnotations = AnnotationsDisplayKind.None,
				ShowParametersType = false,
				ShowQualifiers = QualifierDisplays.None,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForParameterInfo([NotNull] IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations)
			=> new PresenterOptions {
				ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = true,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementAnnotations = AnnotationsDisplayKind.None,
				ShowElementType = ElementTypeDisplay.After,
				ShowEmptyParametersText = settings.GetValue((ParameterInfoSettings s) => s.ShowEmptyParametersText),
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = false,
				ShowParametersName = true,
				ShowParametersAnnotations = showAnnotations,
				ShowParametersType = true,
				ShowQualifiers = QualifierDisplays.None,
				SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseTypeKeywords = settings.GetValue((ParameterInfoSettings s) => s.UseTypeKeywords)
			};

		[NotNull]
		public static PresenterOptions ForBaseTypeOrImplementedInterfaceTooltip([NotNull] IContextBoundSettingsStore settings)
			=> new PresenterOptions {
				ExternalCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.ExternalCodeNamespaceDisplayKind),
				FormatDelegatesAsLambdas = false,
				ShowAccessors = false,
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = true,
				ShowElementKind = ElementKindDisplay.None,
				ShowElementAnnotations = AnnotationsDisplayKind.None,
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersName = false,
				ShowParametersAnnotations = AnnotationsDisplayKind.None,
				ShowParametersType = true,
				ShowQualifiers = QualifierDisplays.Member,
				SolutionCodeNamespaceDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.SolutionCodeNamespaceDisplayKind),
				UseAttributeClassKind = false,
				UseClassModifiersInKind = false,
				UseExtensionMethodKind = false,
				UseMethodModifiersInKind = false,
				UseTypeKeywords = true
			};

		[NotNull]
		public static readonly PresenterOptions FullWithoutParameterNames = new PresenterOptions {
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
			FormatDelegatesAsLambdas = false,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = true,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.Everywhere,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedMember = new PresenterOptions {
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
			FormatDelegatesAsLambdas = false,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.Member,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedName = new PresenterOptions {
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Always,
			FormatDelegatesAsLambdas = false,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = false,
			ShowQualifiers = QualifierDisplays.Member,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Always,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseTypeKeywords = true
		};
		
		[NotNull]
		public static readonly PresenterOptions ForInterfaceMember = new PresenterOptions {
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
			FormatDelegatesAsLambdas = false,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = true,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.None,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions NameOnly = new PresenterOptions {
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
			FormatDelegatesAsLambdas = false,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = false,
			ShowQualifiers = QualifierDisplays.None,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions ParameterTypesOnly = new PresenterOptions {
			ExternalCodeNamespaceDisplayKind = ExternalCodeNamespaceDisplayKind.Never,
			FormatDelegatesAsLambdas = false,
			ShowAccessors = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementKind = ElementKindDisplay.None,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = false,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			ShowQualifiers = QualifierDisplays.None,
			SolutionCodeNamespaceDisplayKind = SolutionCodeNamespaceDisplayKind.Never,
			UseAttributeClassKind = false,
			UseClassModifiersInKind = false,
			UseExtensionMethodKind = false,
			UseMethodModifiersInKind = false,
			UseTypeKeywords = true
		};

		private PresenterOptions() {
		}

	}

}