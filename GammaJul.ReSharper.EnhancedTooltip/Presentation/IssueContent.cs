using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class IssueContent : ITooltipContent {

		[CanBeNull]
		public IconId Icon { get; set; }

		public RichText Text { get; set; }

	}

}