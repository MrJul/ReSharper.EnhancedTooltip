using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresenterOptions {

		public bool FormatDelegatesAsLambdas { get; private set; }
		public bool ShowAccessRights { get; private set; }
		public bool ShowConstantValues { get; private set; }
		public bool ShowElementKind { get; private set; }
		public ElementTypeDisplay ShowElementType { get; private set; }
		public bool ShowModifiers { get; private set; }
		public bool ShowName { get; private set; }
		public NamespaceDisplays ShowNamespaces { get; private set; }
		public bool ShowParameterNames { get; private set; }
		public bool ShowParameterTypes { get; private set; }
		public bool ShowEmptyParametersText { get; private set; }
		public bool UseReSharperColors { get; private set; }
		public bool UseTypeKeywords { get; private set; }

		[NotNull]
		public static PresenterOptions ForToolTip([NotNull] IContextBoundSettingsStore settings) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValues = true,
				ShowElementKind = false,
				ShowElementType = ElementTypeDisplay.Before,
				ShowEmptyParametersText = false,
				ShowModifiers = false,
				ShowName = true,
				ShowNamespaces = NamespaceDisplays.Member,
				ShowParameterTypes = true,
				ShowParameterNames = true,
				UseReSharperColors = settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled),
				UseTypeKeywords = true
			};
		}

		[NotNull]
		public static PresenterOptions ForParameterInfo([NotNull] IContextBoundSettingsStore settings) {
			return new PresenterOptions {
				FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas),
				ShowAccessRights = false,
				ShowConstantValues = true,
				ShowElementKind = false,
				ShowElementType = ElementTypeDisplay.After,
				ShowEmptyParametersText = true,
				ShowModifiers = false,
				ShowName = false,
				ShowNamespaces = NamespaceDisplays.None,
				ShowParameterTypes = true,
				ShowParameterNames = true,
				UseReSharperColors = settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled),
				UseTypeKeywords = true
			};
		}

		private PresenterOptions() {
		}

	}

}