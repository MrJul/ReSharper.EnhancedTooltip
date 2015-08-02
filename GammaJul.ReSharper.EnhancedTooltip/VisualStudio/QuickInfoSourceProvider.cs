using System;
using System.ComponentModel.Composition;
using JetBrains.Annotations;
using JetBrains.Util.Logging;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[Name("EnhancedTooltipQuickInfo")]
	[Order(After = "ReSharperQuickInfo")]
	[ContentType("text")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public sealed class QuickInfoSourceProvider : IQuickInfoSourceProvider {

		[CanBeNull]
		[Import(AllowDefault = true)]
		public IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService { get; set; }
		
		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
			var vsEditorAdaptersFactoryService = VsEditorAdaptersFactoryService;
			if (vsEditorAdaptersFactoryService == null || textBuffer is IProjectionBufferBase)
				return null;

			try {
				return new QuickInfoSource(vsEditorAdaptersFactoryService, textBuffer);
			}
			catch (Exception ex) {
				Logger.LogException("Problem while showing an Enhanced Tooltip.", ex);
				return null;
			}
		}

	}

}