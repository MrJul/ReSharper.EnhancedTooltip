using JetBrains.Annotations;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IssueTooltipContent : ReSharperTooltipContent {

		[CanBeNull]
		public IconId Icon { get; set; }

	}

}