using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class PresentedInfo {

		private readonly List<TextRange> _parameters = new List<TextRange>();
		
		[NotNull]
		public List<TextRange> Parameters {
			get { return _parameters; }
		}

		public bool IsExtensionMethod { get; set; }

	}

}