using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IdentifierTooltipContent : TooltipContent {

		[CanBeNull]
		public IconId Icon { get; set; }
		
		[CanBeNull]
		public RichText Description { get; set; }

		[CanBeNull]
		public RichText Obsolete { get; set; }

		[CanBeNull]
		public RichText Return { get; set; }

		public int? OverloadCount { get; set; }

		[NotNull]
		public List<ExceptionContent> Exceptions { get; } = new List<ExceptionContent>();

		[CanBeNull]
		public RichText BaseType { get; set; }

		[NotNull]
		public List<RichText> ImplementedInterfaces { get; } = new List<RichText>();

		[CanBeNull]
		public AttributeUsageContent AttributeUsage { get; set; }

		public IdentifierTooltipContent([CanBeNull] RichText text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}