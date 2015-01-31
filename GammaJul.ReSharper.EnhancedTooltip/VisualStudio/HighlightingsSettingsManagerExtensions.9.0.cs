using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	internal static class HighlightingsSettingsManagerExtensions {

		public static Severity GetSeverity([NotNull] this HighlightingSettingsManager manager, [NotNull] IHighlighting highlighting,
			[CanBeNull] IDocument document, [CanBeNull] ISolution solution) {
			if (solution == null)
				return Severity.INVALID_SEVERITY;
			return manager.GetSeverity(highlighting, solution);
		}

	}

}