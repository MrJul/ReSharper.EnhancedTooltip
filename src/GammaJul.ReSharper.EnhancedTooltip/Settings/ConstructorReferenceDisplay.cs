using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum ConstructorReferenceDisplay {
		[Description("Constructor")] ConstructorOnly = 0,
		[Description("Type")] TypeOnly = 1,
		[Description("Constructor and type")] Both = 2
	}

}