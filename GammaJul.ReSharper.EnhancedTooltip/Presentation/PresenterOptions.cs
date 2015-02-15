using JetBrains.ReSharper.Feature.Services.Lookup;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresenterOptions {

		public bool FormatDelegatesAsLambdas { get; private set; }
		public bool ShowAccessRights { get; private set; }
		public bool ShowConstantValues { get; private set; }
		public TypeContainerDisplays ShowTypeContainers { get; private set; }
		public bool ShowElementKind { get; private set; }
		public AnnotationsDisplayKind ShowElementAnnotations { get; private set; }
		public ElementTypeDisplay ShowElementType { get; private set; }
		public bool ShowEmptyParametersText { get; private set; }
		public bool ShowExplicitInterface { get; private set; }
		public bool ShowModifiers { get; private set; }
		public bool ShowName { get; private set; }
		public NamespaceDisplays ShowNamespaces { get; private set; }
		public bool ShowParametersName { get; private set; }
		public AnnotationsDisplayKind ShowParametersAnnotations { get; private set; }
		public bool ShowParametersType { get; private set; }
		public bool UseTypeKeywords { get; private set; }

		[NotNull]
		public static PresenterOptions ForToolTip([NotNull] IContextBoundSettingsStore settings) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey s) => s.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowTypeContainers = TypeContainerDisplays.Member,
				ShowConstantValues = true,
				ShowElementKind = settings.GetValue((IdentifierTooltipSettings s) => s.ShowKind),
				ShowElementAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations),
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = true,
				ShowNamespaces = NamespaceDisplays.Member,
				ShowParametersName = true,
				ShowParametersAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowParametersAnnotations),
				ShowParametersType = true,
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};
		}

		[NotNull]
		public static PresenterOptions ForParameterInfo([NotNull] IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValues = true,
				ShowTypeContainers = TypeContainerDisplays.None,
				ShowElementKind = false,
				ShowElementAnnotations = AnnotationsDisplayKind.None,
				ShowElementType = ElementTypeDisplay.After,
				ShowEmptyParametersText = settings.GetValue((ParameterInfoSettings s) => s.ShowEmptyParametersText),
				ShowExplicitInterface = false,
				ShowModifiers = false,
				ShowName = false,
				ShowNamespaces = NamespaceDisplays.None,
				ShowParametersName = true,
				ShowParametersAnnotations = showAnnotations,
				ShowParametersType = true,
				UseTypeKeywords = settings.GetValue((ParameterInfoSettings s) => s.UseTypeKeywords)
			};
		}

		[NotNull]
		public static readonly PresenterOptions FullWithoutParameterNames = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValues = true,
			ShowTypeContainers = TypeContainerDisplays.Everywhere,
			ShowElementKind = false,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = true,
			ShowModifiers = false,
			ShowName = true,
			ShowNamespaces = NamespaceDisplays.Everywhere,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedMember = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValues = true,
			ShowTypeContainers = TypeContainerDisplays.Member,
			ShowElementKind = false,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowNamespaces = NamespaceDisplays.Member,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions QualifiedName = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValues = true,
			ShowTypeContainers = TypeContainerDisplays.Member,
			ShowElementKind = false,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowNamespaces = NamespaceDisplays.Member,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = false,
			UseTypeKeywords = true
		};
		
		[NotNull]
		public static readonly PresenterOptions ForInterfaceMember = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValues = false,
			ShowTypeContainers = TypeContainerDisplays.None,
			ShowElementKind = false,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.Before,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = true,
			ShowModifiers = false,
			ShowName = true,
			ShowNamespaces = NamespaceDisplays.None,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions NameOnly = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValues = false,
			ShowTypeContainers = TypeContainerDisplays.None,
			ShowElementKind = false,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = true,
			ShowNamespaces = NamespaceDisplays.None,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = false,
			UseTypeKeywords = true
		};

		[NotNull]
		public static readonly PresenterOptions ParameterTypesOnly = new PresenterOptions {
			FormatDelegatesAsLambdas = false,
			ShowAccessRights = false,
			ShowConstantValues = false,
			ShowTypeContainers = TypeContainerDisplays.None,
			ShowElementKind = false,
			ShowElementAnnotations = AnnotationsDisplayKind.None,
			ShowElementType = ElementTypeDisplay.None,
			ShowEmptyParametersText = false,
			ShowExplicitInterface = false,
			ShowModifiers = false,
			ShowName = false,
			ShowNamespaces = NamespaceDisplays.None,
			ShowParametersName = false,
			ShowParametersAnnotations = AnnotationsDisplayKind.None,
			ShowParametersType = true,
			UseTypeKeywords = true
		};

		private PresenterOptions() {
		}

	}

}