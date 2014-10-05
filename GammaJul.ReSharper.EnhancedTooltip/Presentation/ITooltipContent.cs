using JetBrains.Annotations;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public interface ITooltipContent {
		
		[CanBeNull]
		RichText Text { get; }

	}

}