using JetBrains.VsIntegration.Application;
using Microsoft.VisualStudio.ExtensionManager;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[WrapVsInterfaces]
	public class ExposeVsExtensionManager : IExposeVsServices {

		public void Register(VsServiceProviderResolver.VsServiceMap map) {
			map.QueryService<SVsExtensionManager>().As<IVsExtensionManager>();
		}

	}

}