using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class ArgumentRoleTooltipContent : TooltipContent {
		
		[CanBeNull]
		public IconId Icon { get; set; }

		[CanBeNull]
		public RichText Description { get; set; }

		public ArgumentRoleTooltipContent([CanBeNull] RichText text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}