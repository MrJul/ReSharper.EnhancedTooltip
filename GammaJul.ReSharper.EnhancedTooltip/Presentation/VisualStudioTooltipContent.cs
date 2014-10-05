using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class VisualStudioTooltipContent : ITooltipContent {

		[CanBeNull]
		public object InnerContent { get; set; }

		public bool IsEmpty {
			get { return InnerContent == null; }
		}

	}

}