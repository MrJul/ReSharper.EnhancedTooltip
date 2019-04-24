using JetBrains.Annotations;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class ExceptionContent {

		[NotNull]
		public string Exception { get; }

		[CanBeNull]
		public RichText Description { get; set; }

		public ExceptionContent([NotNull] string exception) {
			Exception = exception;
		}

	}

}