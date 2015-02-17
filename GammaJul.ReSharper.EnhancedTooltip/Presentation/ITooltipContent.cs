using JetBrains.Annotations;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public interface ITooltipContent {
		
		TextRange TrackingRange { get; }

		[CanBeNull]
		RichText Text { get; }

	}

}