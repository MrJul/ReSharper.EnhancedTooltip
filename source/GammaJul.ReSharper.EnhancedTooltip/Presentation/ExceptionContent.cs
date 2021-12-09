using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class ExceptionContent {

		public string Exception { get; }

		public RichText? Description { get; set; }

		public ExceptionContent(string exception) {
			Exception = exception;
		}

	}

}