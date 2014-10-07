using JetBrains.VsIntegration.Application;
using Microsoft.VisualStudio.ExtensionManager;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[WrapVsInterfaces]
	internal sealed class ExposeVsExtensionManager : IExposeVsServices {

		public void Register(VsServiceProviderResolver.VsServiceMap map) {
			// IVsExtensionManager from VS11 is not the same one as from VS12.
			// This file is included in both Vs11Only and Vs12Only, but the reference to Microsoft.VisualStudio.ExtensionManager is different!
			if (map.Resolve(typeof(IVsExtensionManager)) == null)
				map.QueryService<SVsExtensionManager>().As<IVsExtensionManager>();
		}

	}

}