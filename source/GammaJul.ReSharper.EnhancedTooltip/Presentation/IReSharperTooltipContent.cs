using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public interface IReSharperTooltipContent {

		RichText? Text { get; }

		TextRange TrackingRange { get; }

	}

}