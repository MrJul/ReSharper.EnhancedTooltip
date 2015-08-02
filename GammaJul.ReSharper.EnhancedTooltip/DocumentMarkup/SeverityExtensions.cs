using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.SolutionAnalysis.UI.Resources;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	public static class SeverityExtensions {

		[Pure]
		public static bool IsIssue(this Severity severity) {
			return severity == Severity.HINT
				|| severity == Severity.SUGGESTION
				|| severity == Severity.WARNING
				|| severity == Severity.ERROR;
		}

		[CanBeNull]
		[Pure]
		public static IconId TryGetIcon(this Severity severity) {
			switch (severity) {
				case Severity.HINT:
					return SolutionAnalysisThemedIcons.SolutionAnalysisHint.Id;
				case Severity.SUGGESTION:
					return SolutionAnalysisThemedIcons.SolutionAnalysisSuggestion.Id;
				case Severity.WARNING:
					return SolutionAnalysisThemedIcons.SolutionAnalysisWarning.Id;
				case Severity.ERROR:
					return SolutionAnalysisThemedIcons.SolutionAnalysisError.Id;
				default:
					return null;
			}
		}

	}

}