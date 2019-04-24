using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[Name(Name)]
	[Order(After = QuickInfoSourceProviderNames.VsSquiggle)]
	[Order(After = QuickInfoSourceProviderNames.VsDefault)]
	[Order(After = QuickInfoSourceProviderNames.VsRoslyn)]
	[Order(After = QuickInfoSourceProviderNames.VsLightBulb)]
	[Order(After = QuickInfoSourceProviderNames.VsSyntactic)]
	[Order(After = QuickInfoSourceProviderNames.VsSemantic)]
	[Order(After = QuickInfoSourceProviderNames.ReSharper)]
	[Order(After = VsSquiggleCollectorQuickInfoSourceProvider.Name)]
	[ContentType("text")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public sealed class MainQuickInfoSourceProvider : QuickInfoSourceProviderBase {

		public const string Name = "EnhancedTooltip." + nameof(MainQuickInfoSourceProvider);

		protected override IQuickInfoSource CreateQuickInfoSource(ITextBuffer textBuffer)
			=> new MainQuickInfoSource(textBuffer);

	}

}