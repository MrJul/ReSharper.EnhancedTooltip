using JetBrains.VsIntegration.Application;
using Microsoft.VisualStudio.ExtensionManager;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[WrapVsInterfaces]
	internal sealed class ExposeVsExtensionManager : IExposeVsServices {

		public void Register(VsServiceProviderResolver.VsServiceMap map) {
			if (map.Resolve(typeof(IVsExtensionManager)) == null)
				map.QueryService<SVsExtensionManager>().As<IVsExtensionManager>();
		}

	}

}