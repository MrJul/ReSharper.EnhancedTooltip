using JetBrains.Annotations;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public abstract class TooltipContent : ITooltipContent {

		public RichText Text { get; }

		public TextRange TrackingRange { get; }

		protected TooltipContent([CanBeNull] RichText text, TextRange trackingRange) {
			Text = text;
			TrackingRange = trackingRange;
		}

	}

}