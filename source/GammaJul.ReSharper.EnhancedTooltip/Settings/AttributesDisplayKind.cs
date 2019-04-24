using System.ComponentModel;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public enum AttributesDisplayKind {
		[Description("Never")] Never = 0,
		[Description("Only [NotNull] and [CanBeNull]")] NullnessAnnotations = 1,
		[Description("Only [NotNull], [CanBeNull], [ItemNotNull] and [ItemCanBeNull]")] NullnessAndItemNullnessAnnotations = 2,
		[Description("Only ReSharper annotations")] AllAnnotations = 3,
		[Description("Always")] Always = 4
	}

}