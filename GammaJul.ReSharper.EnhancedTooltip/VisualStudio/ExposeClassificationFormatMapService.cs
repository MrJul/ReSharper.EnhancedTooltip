//using JetBrains.VsIntegration.Application;
//using JetBrains.VsIntegration.SinceVs10.Interop;
//using Microsoft.VisualStudio.Text.Classification;

//namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

//	[WrapVsInterfaces]
//	internal sealed class ExposeClassificationFormatMapService : IExposeVsServices {

//		public void Register(VsServiceProviderResolver.VsServiceMap map) {
//			if (map.Resolve(typeof(IClassificationFormatMapService), VsServiceProviderResolver.VsServiceFlags.IsLazyOnly) == null)
//				map.Mef<IClassificationFormatMapService>();
//		}

//	}

//}