using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Zones;


namespace GammaJul.ReSharper.EnhancedTooltip {

	[ZoneMarker]
	public class ZoneMarker : IRequire<ISinceVs10FrontEnvZone> {
	}

}