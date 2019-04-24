using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum TooltipColorSource {
		[Description("Use Text Editor settings")] TextEditorSettings = 0,
		[Description("Use Environment settings")] EnvironmentSettings = 1
	}

}