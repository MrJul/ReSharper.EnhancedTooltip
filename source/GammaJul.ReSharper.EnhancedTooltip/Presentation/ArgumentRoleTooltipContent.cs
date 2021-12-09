using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class ArgumentRoleTooltipContent : TooltipContent {
		
		public IconId? Icon { get; set; }

		public RichText? Description { get; set; }

		public ArgumentRoleTooltipContent(RichText? text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}