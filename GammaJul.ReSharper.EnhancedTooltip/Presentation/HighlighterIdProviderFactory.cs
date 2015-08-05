using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.VsIntegration.Shell;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[SolutionComponent]
	public class HighlighterIdProviderFactory {

		private readonly bool _useRoslynColors;

		[NotNull]
		public HighlighterIdProvider CreateProvider([NotNull] IContextBoundSettingsStore settings)
			=> new HighlighterIdProvider(settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled), _useRoslynColors);

		public HighlighterIdProviderFactory([NotNull] IVsEnvironmentInformation vsEnvironment) {
			_useRoslynColors = vsEnvironment.VsVersion2.Major >= 14;
		}

	}

}