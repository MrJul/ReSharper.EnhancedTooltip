using System;
using JetBrains.Annotations;
using JetBrains.Util.Logging;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Projection;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
	public sealed partial class QuickInfoSourceProvider : IQuickInfoSourceProvider {

		[CanBeNull]
		private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
			if (_editorAdaptersFactoryService == null || textBuffer is IProjectionBufferBase)
				return null;

			try {
				return new QuickInfoSource(_editorAdaptersFactoryService, textBuffer);
			}
			catch (Exception ex) {
				Logger.LogException("Problem while showing an Enhanced Tooltip.", ex);
				return null;
			}
		}

		public QuickInfoSourceProvider([CanBeNull] IVsEditorAdaptersFactoryService editorAdaptersFactoryService) {
			_editorAdaptersFactoryService = editorAdaptersFactoryService;
		}

	}

}