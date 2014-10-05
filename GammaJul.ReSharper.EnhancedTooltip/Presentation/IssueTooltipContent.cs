using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IssueTooltipContent : ITooltipContent {

		public RichText Text { get; set; }

		[CanBeNull]
		public IconId Icon { get; set; }

	}

}