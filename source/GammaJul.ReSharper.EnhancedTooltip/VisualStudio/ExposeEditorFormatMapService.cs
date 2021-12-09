using JetBrains.Platform.VisualStudio.SinceVs10.Interop;
using JetBrains.VsIntegration.Shell;
using Microsoft.VisualStudio.Text.Classification;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[WrapVsInterfaces]
	public class ExposeEditorFormatMapService : IExposeVsServices {
		
		public void Register(VsServiceProviderResolver.VsServiceMap map)
			=> map.Mef<IEditorFormatMapService>();

	}

}