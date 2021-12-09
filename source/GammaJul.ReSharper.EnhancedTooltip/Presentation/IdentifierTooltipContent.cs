using System.Collections.Generic;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IdentifierTooltipContent : TooltipContent {

		public IconId? Icon { get; set; }
		
		public RichText? Description { get; set; }

		public RichText? Obsolete { get; set; }

		public RichText? Return { get; set; }

		public RichText? Value { get; set; }

		public RichText? Remarks { get; set; }

		public int? OverloadCount { get; set; }

		public List<ExceptionContent> Exceptions { get; } = new();

		public RichText? BaseType { get; set; }

		public List<RichText> ImplementedInterfaces { get; } = new();

		public AttributeUsageContent? AttributeUsage { get; set; }

		public IdentifierTooltipContent(RichText? text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}