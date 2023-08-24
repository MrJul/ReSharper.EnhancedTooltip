using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class ExceptionContent {

		public RichText? Exception { get; }

		public RichText? Description { get; set; }

		public ExceptionContent(RichText? exception) {
			Exception = exception;
		}

	}

}