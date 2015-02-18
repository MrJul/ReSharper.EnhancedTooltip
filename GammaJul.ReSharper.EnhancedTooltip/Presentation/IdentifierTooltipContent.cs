using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IdentifierTooltipContent : TooltipContent {

		[NotNull] private readonly IList<ExceptionContent> _exceptions = new List<ExceptionContent>();

		[CanBeNull]
		public IconId Icon { get; set; }
		
		[CanBeNull]
		public RichText Description { get; set; }

		[CanBeNull]
		public RichText Obsolete { get; set; }

		public int? OverloadCount { get; set; }

		[NotNull]
		public IList<ExceptionContent> Exceptions {
			get { return _exceptions; }
		}

		public IdentifierTooltipContent([CanBeNull] RichText text, TextRange trackingRange)
			: base(text, trackingRange) {
		}

	}

}