using System;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[Flags]
	public enum TypeContainerDisplays {
		None = 0x0,
		Member = 0x1,
		ElementType = 0x2,
		TypeParameters = 0x4,
		Parameters = 0x8,
		Everywhere = Member | ElementType | TypeParameters | Parameters
	}

}