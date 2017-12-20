using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum SolutionCodeNamespaceDisplayKind {
		[Description("Always")] Always = 0,
		[Description("Never")] Never = 1,
		[Description("Smart (removes current namespace)")] Smart = 2
	}

}