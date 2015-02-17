using JetBrains.Annotations;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class MiscTooltipContent : TooltipContent {

		public MiscTooltipContent([CanBeNull] RichText text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}