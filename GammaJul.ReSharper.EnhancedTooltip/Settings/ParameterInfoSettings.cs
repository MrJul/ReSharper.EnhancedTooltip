using JetBrains.Application.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how information about parameters are shown when writing a call.")]
	public class ParameterInfoSettings {

		[SettingsEntry(true, "Use type keywords when possible (eg. int instead of Int32).")]
		public bool UseTypeKeywords { get; set; }

		[SettingsEntry(true, "Display [CanBeNull] and [NotNull] for parameters inside calls.")]
		public bool ShowParametersNullness { get; set; }

	}

}