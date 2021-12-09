using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public abstract class TooltipContent : IReSharperTooltipContent {

		public RichText? Text { get; }

		public TextRange TrackingRange { get; }

		protected TooltipContent(RichText? text, TextRange trackingRange) {
			Text = text;
			TrackingRange = trackingRange;
		}

	}

}