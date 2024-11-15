using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[Name(Name)]
  [Order(After = QuickInfoSourceProviderNames.VsDefault)]
  [Order(After = QuickInfoSourceProviderNames.VsDefault2)]
  [Order(After = QuickInfoSourceProviderNames.VsRoslyn)]
  [Order(After = QuickInfoSourceProviderNames.VsSyntactic)]
  [Order(After = QuickInfoSourceProviderNames.VsSemantic)]
  [Order(After = QuickInfoSourceProviderNames.VsSquiggle)]
  [Order(After = QuickInfoSourceProviderNames.VsLightBulb)]
	[Order(After = QuickInfoSourceProviderNames.ReSharper)]
	[Order(Before = MainQuickInfoSourceProvider.Name)]
	[ContentType("any")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public sealed class VsSquiggleCollectorQuickInfoSourceProvider : QuickInfoSourceProviderBase {

		public const string Name = "EnhancedTooltip." + nameof(VsSquiggleCollectorQuickInfoSourceProvider);

		protected override IQuickInfoSource CreateQuickInfoSource(ITextBuffer textBuffer)
			=> new VsSquiggleCollectorQuickInfoSource(textBuffer);

	}

}