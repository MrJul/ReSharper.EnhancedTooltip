using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum ExternalCodeNamespaceDisplayKind {
		[Description("Always")] Always = 0,
		[Description("Never")] Never = 1,
		[Description("Only for non System-namespaces")] OnlyForNonSystem = 2
	}

}