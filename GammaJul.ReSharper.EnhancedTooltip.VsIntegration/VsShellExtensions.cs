using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace GammaJul.ReSharper.EnhancedTooltip.VsIntegration {

	internal static class VsShellExtensions {

		private static readonly Guid _reSharperPackageGuid = new Guid("0c6e6407-13fc-4878-869a-c8b4016c57fe");

		private static bool IsPackageInstalled(IVsShell vsShell, Guid packageGuid) {
			int installed;
			return vsShell.IsPackageInstalled(ref packageGuid, out installed) == VSConstants.S_OK
				&& installed != 0;
		}

		private static IVsPackage TryLoadPackage(IVsShell vsShell, Guid packageGuid) {
			IVsPackage package;
			if (vsShell.IsPackageLoaded(ref packageGuid, out package) == VSConstants.S_OK && package != null)
				return package;

			if (vsShell.LoadPackage(ref packageGuid, out package) == VSConstants.S_OK)
				return package;

			return null;
		}

		public static Version TryGetReSharperVersion(this IVsShell vsShell) {
			if (vsShell == null || !IsPackageInstalled(vsShell, _reSharperPackageGuid))
				return null;

			IVsPackage vsPackage = TryLoadPackage(vsShell, _reSharperPackageGuid);
			if (vsPackage == null)
				return null;

			return vsPackage.GetType().Assembly.GetName().Version;
		}

	}

}