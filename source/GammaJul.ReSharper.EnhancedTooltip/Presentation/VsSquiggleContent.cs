using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class VsSquiggleContent {

		[NotNull]
		public string Content { get; }

		public VsSquiggleContent([NotNull] string content) {
			Content = content;
		}

	}

}