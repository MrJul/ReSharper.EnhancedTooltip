using JetBrains.VsIntegration.Application;
using JetBrains.VsIntegration.SinceVs10.Interop;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Text.Classification;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[WrapVsInterfaces]
	internal sealed class ExposeVsServices : IExposeVsServices {

		public void Register(VsServiceProviderResolver.VsServiceMap map) {
			if (map.Resolve(typeof(IVsExtensionManager)) == null)
				map.QueryService<SVsExtensionManager>().As<IVsExtensionManager>();

			if (map.Resolve(typeof(IClassificationFormatMapService)) == null)
				map.Mef<IClassificationFormatMapService>();
		}

	}

}