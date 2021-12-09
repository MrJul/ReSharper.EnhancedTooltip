using System.Collections.Generic;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class PresentedInfo {

		public List<TextRange> Parameters { get; } = new();

		public List<TextRange> TypeParameters { get; } = new();

		public bool IsExtensionMethod { get; set; }

	}

}