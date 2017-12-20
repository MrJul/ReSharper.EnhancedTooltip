using System;
using System.ComponentModel.Composition;
using JetBrains.Annotations;
using JetBrains.Util.Logging;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Projection;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public abstract class QuickInfoSourceProviderBase : IQuickInfoSourceProvider {

		[CanBeNull]
		[Import(AllowDefault = true)]
		public IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService { get; set; }
		
		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
			var vsEditorAdaptersFactoryService = VsEditorAdaptersFactoryService;
			if (vsEditorAdaptersFactoryService == null || textBuffer is IProjectionBufferBase)
				return null;

			try {
				return CreateQuickInfoSource(vsEditorAdaptersFactoryService, textBuffer);
			}
			catch (Exception ex) {
				Logger.LogException($"Problem while creating a quick info source for {GetType().FullName}", ex);
				return null;
			}
		}

		[NotNull]
		protected abstract IQuickInfoSource CreateQuickInfoSource(
			[NotNull] IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService,
			[NotNull] ITextBuffer textBuffer);

	}

}