using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.ParameterInfo.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class PresenterOptions {

		public bool FormatDelegatesAsLambdas { get; set; }
		public bool ShowAccessRights { get; set; }
		public bool ShowConstantValues { get; set; }
		public bool ShowElementKind { get; set; }
		public ElementTypeDisplay ShowElementType { get; set; }
		public bool ShowModifiers { get; set; }
		public bool ShowName { get; set; }
		public NamespaceDisplays ShowNamespaces { get; set; }
		public bool ShowParameterNames { get; set; }
		public bool ShowParameterTypes { get; set; }
		public bool ShowEmptyParametersText { get; set; }
		public bool UseReSharperColors { get; set; }
		public bool UseTypeKeywords { get; set; }

		public PresenterOptions([NotNull] IContextBoundSettingsStore settings) {
			FormatDelegatesAsLambdas = settings.GetValue((ParameterInfoSettingsKey key) => key.DelegatesAsLambdas);
			ShowAccessRights = false;
			ShowConstantValues = true;
			ShowElementKind = false;
			ShowElementType = ElementTypeDisplay.Before;
			ShowEmptyParametersText = false;
			ShowModifiers = false;
			ShowName = true;
			ShowNamespaces = NamespaceDisplays.Member;
			ShowParameterTypes = true;
			ShowParameterNames = true;
			UseReSharperColors = settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled);
			UseTypeKeywords = true;
		}

	}

}