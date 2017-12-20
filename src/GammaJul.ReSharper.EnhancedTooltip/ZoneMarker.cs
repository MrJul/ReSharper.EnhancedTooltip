using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs11.Shell.Zones;

namespace GammaJul.ReSharper.EnhancedTooltip {

	[ZoneMarker]
	public class ZoneMarker : IRequire<ISinceVs11Zone> {
	}

}