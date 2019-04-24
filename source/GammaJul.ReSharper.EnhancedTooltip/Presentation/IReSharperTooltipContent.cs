using JetBrains.Annotations;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public interface IReSharperTooltipContent {

		[CanBeNull]
		RichText Text { get; }

		TextRange TrackingRange { get; }

	}

}