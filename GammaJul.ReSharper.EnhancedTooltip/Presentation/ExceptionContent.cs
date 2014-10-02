using JetBrains.Annotations;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class ExceptionContent {

		[CanBeNull]
		public string Exception { get; set; }

		[CanBeNull]
		public RichText Description { get; set; }

	}

}