using System.Windows.Media;
using JetBrains.Application.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how tooltips are displayed globally")]
	public class DisplaySettings {
		
		[SettingsEntry(true, "Limit tooltip width to percentage of screen width")]
		public bool LimitTooltipWidth { get; set; }

		[SettingsEntry(50, "Screen width limit (%)")]
		public int ScreenWidthLimitPercent { get; set; }

		[SettingsEntry(TextFormattingMode.Ideal, "Text display mode")]
		public TextFormattingMode TextFormattingMode { get; set; }

	}

}