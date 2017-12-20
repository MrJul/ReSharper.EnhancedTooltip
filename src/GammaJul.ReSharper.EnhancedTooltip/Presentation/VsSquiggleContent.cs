using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class VsSquiggleContent {

		[NotNull]
		public object Content { get; }

		public VsSquiggleContent([NotNull] object content) {
			Content = content;
		}

	}

}