using System;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class ParamContent {

		public RichText Name { get; }

		public RichText? Description { get; set; }

		public Boolean IsTypeParameter { get; set; }

		public ParamContent(RichText name) {
      this.Name = name;
		}

	}

}