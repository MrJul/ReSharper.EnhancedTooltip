using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs11.Shell.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ReSharper.EnhancedTooltip {

	[ZoneMarker]
	public class ZoneMarker : IRequire<ISinceVs11Zone>, IRequire<ICodeEditingZone>, IRequire<DaemonZone> {
	}

}