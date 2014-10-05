using JetBrains.Annotations;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class ReSharperTooltipContent : ITooltipContent {

		[CanBeNull]
		public RichText Text { get; set; }

		public bool IsEmpty {
			get { return Text.IsNullOrEmpty(); }
		}

	}

}