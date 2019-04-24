using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.VsIntegration.Shell;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[SolutionComponent]
	public class HighlighterIdProviderFactory {

		private readonly uint _vsMajorVersion;

		[NotNull]
		public HighlighterIdProvider CreateProvider([NotNull] IContextBoundSettingsStore settings)
			=> new HighlighterIdProvider(GetHighlighterIdSource(settings));

		private HighlighterIdSource GetHighlighterIdSource([NotNull] IContextBoundSettingsStore settings) {
			if (settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled))
				return HighlighterIdSource.ReSharper;
			if (_vsMajorVersion >= 16u)
				return HighlighterIdSource.VisualStudio16;
			if (_vsMajorVersion >= 14u)
				return HighlighterIdSource.VisualStudio14;
			return HighlighterIdSource.VisualStudioLegacy;
		}

		public HighlighterIdProviderFactory([NotNull] IVsEnvironmentInformation vsEnvironment) {
			_vsMajorVersion = vsEnvironment.VsVersion2.Major;
		}

	}

}