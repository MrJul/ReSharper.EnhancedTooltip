using JetBrains.Annotations;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public abstract class TooltipContent : ITooltipContent {

		[CanBeNull] private readonly RichText _text;
		private readonly TextRange _trackingRange;

		public RichText Text {
			get { return _text; }
		}

		public TextRange TrackingRange {
			get { return _trackingRange; }
		}

		protected TooltipContent([CanBeNull] RichText text, TextRange trackingRange) {
			_text = text;
			_trackingRange = trackingRange;
		}

	}

}