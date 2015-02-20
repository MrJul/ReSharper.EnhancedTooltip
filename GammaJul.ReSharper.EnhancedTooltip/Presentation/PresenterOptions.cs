using JetBrains.ReSharper.Feature.Services.Lookup;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresenterOptions {

		public bool FormatDelegatesAsLambdas { get; private set; }
		public bool ShowAccessRights { get; private set; }
		public bool ShowConstantValue { get; private set; }
		public bool ShowDefaultValues { get; private set; }
		public bool ShowElementKind { get; private set; }
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
		public bool UseTypeKeywords { get; private set; }

		[NotNull]
		public static PresenterOptions ForToolTip([NotNull] IContextBoundSettingsStore settings) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey s) => s.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValue = true,
				ShowDefaultValues = true,
				ShowElementKind = settings.GetValue((IdentifierTooltipSettings s) => s.ShowKind),
				ShowElementAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations),
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowParametersName = true,
				ShowParametersAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowParametersAnnotations),
				ShowParametersType = true,
				ShowQualifiers = QualifierDisplays.Member,
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};
		}

		[NotNull]
		public static PresenterOptions ForParameterInfo([NotNull] IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValue = false,
				ShowDefaultValues = true,
				ShowElementKind = false,
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
				UseTypeKeywords = settings.GetValue((ParameterInfoSettings s) => s.UseTypeKeywords)
			};
		}

		[NotNull]
		public static readonly PresenterOptions FullWithoutParameterNames = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementKind = false,
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
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedMember = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementKind = false,
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
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedName = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = true,
			ShowElementKind = false,
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
			UseTypeKeywords = true
		};
		
		[NotNull]
		public static readonly PresenterOptions ForInterfaceMember = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementKind = false,
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
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions NameOnly = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementKind = false,
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
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions ParameterTypesOnly = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValue = false,
			ShowDefaultValues = false,
			ShowElementKind = false,
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
			UseTypeKeywords = true
		};

		private PresenterOptions() {
		}

	}

}