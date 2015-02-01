using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[Name("EnhancedTooltipQuickInfo")]
	[Order(After = "ReSharperQuickInfo")]
	[ContentType("text")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public partial class QuickInfoSourceProvider {
	}

}