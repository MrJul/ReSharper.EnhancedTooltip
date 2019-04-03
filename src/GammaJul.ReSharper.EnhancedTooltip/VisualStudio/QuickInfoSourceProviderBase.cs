using JetBrains.Annotations;
using JetBrains.Util.Logging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Projection;
using System;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public abstract class QuickInfoSourceProviderBase : IQuickInfoSourceProvider {

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
			if (textBuffer is IProjectionBufferBase)
				return null;

			try {
				return CreateQuickInfoSource(textBuffer);
			}
			catch (Exception ex) {
				Logger.LogException($"Problem while creating a quick info source for {GetType().FullName}", ex);
				return null;
			}
		}

		[NotNull]
		protected abstract IQuickInfoSource CreateQuickInfoSource([NotNull] ITextBuffer textBuffer);

	}

}