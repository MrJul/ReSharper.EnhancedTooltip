using System;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[Flags]
	public enum QualifierDisplays {
		None = 0,
		Member = 1 << 0,
		ElementType = 1 << 1,
		TypeParameters = 1 << 2,
		Parameters = 1 << 3,
		Everywhere = Member | ElementType | TypeParameters | Parameters
	}

}