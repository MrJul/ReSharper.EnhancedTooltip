using JetBrains.Application.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how tooltips are shown for issues")]
	public class IssueTooltipSettings {

		[SettingsEntry(true, "Display an icon for issues")]
		public bool ShowIcon { get; set; }

		[SettingsEntry(true, "Colorize elements in errors when possible")]
		public bool ColorizeElementsInErrors { get; set; }

	}

}