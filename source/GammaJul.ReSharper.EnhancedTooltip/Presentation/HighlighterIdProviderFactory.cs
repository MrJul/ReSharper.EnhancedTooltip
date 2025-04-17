using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util.DevEnv;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
	public class HighlighterIdProviderFactory {

		private readonly uint _vsMajorVersion;

		public HighlighterIdProvider CreateProvider(IContextBoundSettingsStore settings)
			=> new HighlighterIdProvider(GetHighlighterIdSource(settings));

		private HighlighterIdSource GetHighlighterIdSource(IContextBoundSettingsStore settings) {
			if (settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled))
				return HighlighterIdSource.ReSharper;
			if (_vsMajorVersion >= 16u)
				return HighlighterIdSource.VisualStudio16;
			if (_vsMajorVersion >= 14u)
				return HighlighterIdSource.VisualStudio14;
			return HighlighterIdSource.VisualStudioLegacy;
		}

		public HighlighterIdProviderFactory(IVsEnvironmentInformation vsEnvironment)
			=> _vsMajorVersion = vsEnvironment.VsVersion2.Major;

	}

}