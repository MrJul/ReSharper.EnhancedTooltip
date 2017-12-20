using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class VsIdentifierContent {

		[NotNull]
		public object Content { get; }

		public VsIdentifierContent([NotNull] object content) {
			Content = content;
		}

	}

}