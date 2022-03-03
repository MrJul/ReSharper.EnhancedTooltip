using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class ParamContent {

		public string Name { get; }

		public RichText? Description { get; set; }

		public ParamContent(string name) {
			Name = name;
		}

	}

}