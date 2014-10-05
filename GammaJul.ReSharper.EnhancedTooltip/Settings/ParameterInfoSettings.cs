using JetBrains.Application.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how information about parameters are shown when writing a call.")]
	public class ParameterInfoSettings {

		[SettingsEntry(true, "Enhance parameter information popups.")]
		public bool Enabled { get; set; }

		[SettingsEntry(true, "Colorize C# calls.")]
		public bool ColorizeSupportedLanguages { get; set; }

		[SettingsEntry(true, "Display <no parameters> when there are no parameters.")]
		public bool ShowEmptyParametersText { get; set; }

		[SettingsEntry(true, "Use type keywords (eg. int instead of Int32).")]
		public bool UseTypeKeywords { get; set; }

		[SettingsEntry(true, "Display [CanBeNull] and [NotNull].")]
		public bool ShowNullness { get; set; }

	}

}