using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	public sealed class TooltipContent {

		[CanBeNull]
		public RichText MainText { get; set; }

		[CanBeNull]
		public RichText DescriptionText { get; set; }

		[CanBeNull]
		public IconId Icon { get; set; }


	}

}