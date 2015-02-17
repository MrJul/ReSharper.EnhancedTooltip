using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IssueTooltipContent : TooltipContent {

		[CanBeNull]
		public IconId Icon { get; set; }

		public IssueTooltipContent([CanBeNull] RichText text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}