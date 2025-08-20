using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Zones;
using JetBrains.TextControl;


namespace GammaJul.ReSharper.EnhancedTooltip {

	[ZoneMarker]
	public class ZoneMarker : IRequire<ISinceVs10FrontEnvZone>, IRequire<ITextControlsZone> {
	}

}