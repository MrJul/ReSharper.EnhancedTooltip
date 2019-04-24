using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum BaseTypeDisplayKind {
		[Description("Never")] Never = 0,
		[Description("Only if in solution code")] SolutionCode = 1,
		[Description("Only if in solution code or in non-System namespace")] SolutionCodeAndNonSystemExternalCode = 2,
		[Description("Only if not System.Object")] OnlyIfNotSystemObject = 3,
		[Description("Always")] Always = 4
	}

}