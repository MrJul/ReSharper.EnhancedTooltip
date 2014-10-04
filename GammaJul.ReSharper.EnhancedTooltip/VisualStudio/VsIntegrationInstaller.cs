using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Application.Extensions;
using JetBrains.Util;
using Microsoft.VisualStudio.ExtensionManager;
using IExtension = JetBrains.Application.Extensions.IExtension;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	/// <summary>
	/// Installs the Enhanced Tooltip VSIX package necessary to make the extension work correctly.
	/// </summary>
	[ShellComponent]
	public class VsIntegrationInstaller : IExtensionRepository {

		private const string ExtensionId = "EnhancedTooltip";

		private readonly Optional<IVsExtensionManager> _vsExtensionManager;

		private bool _shouldRestart;

		public bool CanUninstall(string id) {
			return false;
		}

		public bool HasMissingExtensions() {
			return false;
		}

		public IEnumerable<string> GetExtensionsRequiringRestart() {
			return _shouldRestart ? new[] { ExtensionId } : EmptyList<string>.InstanceList;
		}

		public void Uninstall(string id, bool removeDependencies, IEnumerable<string> dependencies, Action<LoggingLevel, string> logger) {
			if (id != ExtensionId)
				return;

			IVsExtensionManager vsExtensionManager = _vsExtensionManager.CanBeNull;
			if (vsExtensionManager == null)
				return;

			IInstalledExtension thisExtension;
			if (!vsExtensionManager.TryGetInstalledExtension("GammaJul.ReSharper.EnhancedTooltip.VsIntegration", out thisExtension))
				return;

			try {
				vsExtensionManager.Uninstall(thisExtension);
			}
			catch (Exception ex) {
				logger(LoggingLevel.WARN, "Could not uninstall EnhancedTooltip VS integration: {0}".FormatEx(ex));
			}
		}

		public void RestoreMissingExtensions() {
		}

		private void TryInstallVsIntegration([CanBeNull] IExtension extension, [CanBeNull] IVsExtensionManager vsExtensionManager) {
			if (extension == null || vsExtensionManager == null)
				return;

			FileSystemPath vsixPath = extension.GetFiles("vsix").FirstOrDefault();
			if (vsixPath == null)
				return;

			IInstallableExtension installableExtension = vsExtensionManager.CreateInstallableExtension(vsixPath.FullPath);
			if (vsExtensionManager.Install(installableExtension, false) != RestartReason.None)
				_shouldRestart = true;
		}

		public VsIntegrationInstaller([NotNull] ExtensionManager extensionManager, [NotNull] Optional<IVsExtensionManager> vsExtensionManager) {
			_vsExtensionManager = vsExtensionManager;
			
			TryInstallVsIntegration(extensionManager.GetExtension(ExtensionId), vsExtensionManager.CanBeNull);
		}

	}

}