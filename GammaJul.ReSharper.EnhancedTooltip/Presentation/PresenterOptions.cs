using JetBrains.ReSharper.Feature.Services.Lookup;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresenterOptions {

		public bool FormatDelegatesAsLambdas { get; private set; }
		public bool ShowAccessRights { get; private set; }
		public bool ShowConstantValues { get; private set; }
		public bool ShowElementKind { get; private set; }
		public AnnotationsDisplayKind ShowElementAnnotations { get; private set; }
		public ElementTypeDisplay ShowElementType { get; private set; }
		public bool ShowModifiers { get; private set; }
		public bool ShowName { get; private set; }
		public NamespaceDisplays ShowNamespaces { get; private set; }
		public bool ShowParametersName { get; private set; }
		public AnnotationsDisplayKind ShowParametersAnnotations { get; private set; }
		public bool ShowParametersType { get; private set; }
		public bool ShowEmptyParametersText { get; private set; }
		public bool UseReSharperColors { get; private set; }
		public bool UseTypeKeywords { get; private set; }
		
		[NotNull]
		public static PresenterOptions ForToolTip([NotNull] IContextBoundSettingsStore settings) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey s) => s.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValues = true,
				ShowElementKind = settings.GetValue((IdentifierTooltipSettings s) => s.ShowKind),
				ShowElementAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations),
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowModifiers = false,
				ShowName = true,
				ShowNamespaces = NamespaceDisplays.Member,
				ShowParametersName = true,
				ShowParametersAnnotations = settings.GetValue((IdentifierTooltipSettings s) => s.ShowParametersAnnotations),
				ShowParametersType = true,
				UseReSharperColors = settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled),
				UseTypeKeywords = settings.GetValue((IdentifierTooltipSettings s) => s.UseTypeKeywords)
			};
		}

		[NotNull]
		public static PresenterOptions ForParameterInfo([NotNull] IContextBoundSettingsStore settings, AnnotationsDisplayKind showAnnotations) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValues = true,
				ShowElementKind = false,
				ShowElementAnnotations = AnnotationsDisplayKind.None,
				ShowElementType = ElementTypeDisplay.After,
				ShowEmptyParametersText = settings.GetValue((ParameterInfoSettings s) => s.ShowEmptyParametersText),
				ShowModifiers = false,
				ShowName = false,
				ShowNamespaces = NamespaceDisplays.None,
				ShowParametersName = true,
				ShowParametersAnnotations = showAnnotations,
				ShowParametersType = true,
				UseReSharperColors = settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled),
				UseTypeKeywords = settings.GetValue((ParameterInfoSettings s) => s.UseTypeKeywords)
			};
		}

		private PresenterOptions() {
		}

	}

}