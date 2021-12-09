using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IssueTooltipContent : TooltipContent {

		public IconId? Icon { get; set; }

		public IssueTooltipContent(RichText? text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}