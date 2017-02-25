using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class PresentedInfo {

		[NotNull]
		public List<TextRange> Parameters { get; } = new List<TextRange>();

		[NotNull]
		public List<TextRange> TypeParameters { get; } = new List<TextRange>();

		public bool IsExtensionMethod { get; set; }

	}

}