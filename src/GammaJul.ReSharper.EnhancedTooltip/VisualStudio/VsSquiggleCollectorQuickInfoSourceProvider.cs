using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[Name(Name)]
	[Order(After = QuickInfoSourceProviderNames.VsSquiggle)]
	[Order(Before = QuickInfoSourceProviderNames.ReSharper)]
	[Order(Before = MainQuickInfoSourceProvider.Name)]
	[ContentType("text")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public sealed class VsSquiggleCollectorQuickInfoSourceProvider : QuickInfoSourceProviderBase {

		public const string Name = "EnhancedTooltip." + nameof(VsSquiggleCollectorQuickInfoSourceProvider);

		protected override IQuickInfoSource CreateQuickInfoSource(IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService, ITextBuffer textBuffer)
			=> new VsSquiggleCollectorQuickInfoSource(vsEditorAdaptersFactoryService, textBuffer);

	}

}