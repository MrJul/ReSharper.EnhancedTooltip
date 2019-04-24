using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum AttributesFormattingMode {
		[Description("All on current line")] AllOnCurrentLine = 0,
		[Description("All on single new line")] AllOnNewLine = 1,
		[Description("One per line if multiple")] OnePerLineIfMultiple = 2,
		[Description("One per line")] OnePerLine = 3
	}

}