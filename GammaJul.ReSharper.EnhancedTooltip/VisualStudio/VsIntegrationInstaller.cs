using System;
using System.Collections.Generic;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Application.Extensions;
using JetBrains.Util;
using Microsoft.VisualStudio.ExtensionManager;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	/// <summary>
	/// Installs the Enhanced Tooltip VSIX package necessary to make the extension work correctly.
	/// </summary>
	[ShellComponent]
	public class VsIntegrationInstaller : IExtensionRepository {

		private const string ExtensionId = "EnhancedTooltip";
		private const string VsIntegrationExtensionId = "GammaJul.ReSharper.EnhancedTooltip.VsIntegration";

		private readonly Optional<IVsExtensionManager> _vsExtensionManager;
		private bool _restartRequired;

		public bool HasMissingExtensions() {
			return false;
		}

		public void RestoreMissingExtensions() {
		}

		public IEnumerable<string> GetExtensionsRequiringRestart() {
			return _restartRequired ? new[] { ExtensionId } : EmptyList<string>.InstanceList;
		}

		public bool CanUninstall(string id) {
			return id == ExtensionId;
		}

		public void Uninstall(string id, bool removeDependencies, IEnumerable<string> dependencies, Action<LoggingLevel, string> logger) {
			if (id != ExtensionId)
				return;

			IVsExtensionManager vsExtensionManager = _vsExtensionManager.CanBeNull;
			if (vsExtensionManager == null)
				return;

			IInstalledExtension thisExtension;
			if (!vsExtensionManager.TryGetInstalledExtension(VsIntegrationExtensionId, out thisExtension))
				return;

			try {
				vsExtensionManager.Uninstall(thisExtension);
			}
			catch (Exception ex) {
				logger(LoggingLevel.WARN, "Could not uninstall EnhancedTooltip VS integration: {0}".FormatEx(ex));
			}
		}

		private void TryInstallVsIntegration([CanBeNull] IVsExtensionManager vsExtensionManager) {
			if (vsExtensionManager == null)
				return;

			IInstalledExtension vsIntegrationExtension;
			if (vsExtensionManager.TryGetInstalledExtension(VsIntegrationExtensionId, out vsIntegrationExtension))
				return;

			FileSystemPath thisAssemblyPath = FileSystemPath.TryParse(typeof(VsIntegrationInstaller).Assembly.Location);
			if (thisAssemblyPath.IsNullOrEmpty())
				return;

			FileSystemPath vsixPath = thisAssemblyPath.Directory.Combine(VsIntegrationExtensionId + ".vsix");
			if (!vsixPath.ExistsFile) {
				MessageBox.ShowInfo("Does not exist: \"{0}\"".FormatEx(vsixPath.FullPath));
				return;
			}

			IInstallableExtension installableExtension = vsExtensionManager.CreateInstallableExtension(vsixPath.FullPath);
			RestartReason restartReason = vsExtensionManager.Install(installableExtension, false);
			if (restartReason != RestartReason.None)
				_restartRequired = true;
		}

		public VsIntegrationInstaller([NotNull] Optional<IVsExtensionManager> vsExtensionManager) {
			_vsExtensionManager = vsExtensionManager;

			TryInstallVsIntegration(vsExtensionManager.CanBeNull);
		}

	}

}