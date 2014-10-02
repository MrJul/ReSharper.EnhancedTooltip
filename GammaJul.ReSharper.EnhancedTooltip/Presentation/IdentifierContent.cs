using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class IdentifierContent : ITooltipContent {

		private readonly IList<ExceptionContent> _exceptions = new List<ExceptionContent>();
			
		[CanBeNull]
		public IconId Icon { get; set; }

		public RichText Text { get; set; }

		[CanBeNull]
		public RichText Description { get; set; }

		[CanBeNull]
		public RichText Obsolete { get; set; }

		[NotNull]
		public IList<ExceptionContent> Exceptions {
			get { return _exceptions; }
		}

	}

}