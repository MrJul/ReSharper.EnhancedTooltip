using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.SolutionAnalysis.Resources;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.UI.Icons;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	public static class SeverityExtensions {

		[Pure]
		public static bool IsIssue(this Severity severity) {
			return severity switch {
				Severity.HINT or Severity.SUGGESTION or Severity.WARNING or Severity.ERROR => true,
				_ => false
			};
		}

		[Pure]
		public static IconId? TryGetIcon(this Severity severity) {
			return severity switch {
				Severity.HINT => SolutionAnalysisThemedIcons.SolutionAnalysisHint.Id,
				Severity.SUGGESTION => SolutionAnalysisThemedIcons.SolutionAnalysisSuggestion.Id,
				Severity.WARNING => SolutionAnalysisThemedIcons.SolutionAnalysisWarning.Id,
				Severity.ERROR => SolutionAnalysisThemedIcons.SolutionAnalysisError.Id,
				_ => null
			};
		}

	}

}