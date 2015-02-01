using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VsIntegration {

	/// <summary>
	/// This class delegates to the real provider in the main extension assembly if it's available.
	/// This is in a separate assembly without any reference to ReSharper or the main project.
	/// Visual Studio can load it without problems even if Enhanced Tooltip or ReSharper are uninstalled.
	/// Only used in ReSharper 8.2. ReSharper 9 supports exporting MEF components directly.
	/// </summary>
	[Name("EnhancedTooltipQuickInfo")]
	[Order(After = "ReSharperQuickInfo")]
	[ContentType("text")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public sealed class QuickInfoSourceProviderProxy : IQuickInfoSourceProvider {

		private const string ProviderTypeNameWithoutVersion = "GammaJul.ReSharper.EnhancedTooltip.VisualStudio.QuickInfoSourceProvider, GammaJul.ReSharper.EnhancedTooltip.";

		private readonly Lazy<IQuickInfoSourceProvider> _lazyProvider;
		
		[Import(AllowDefault = true)]
		public IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService { get; set; }

		[Import(AllowDefault = true)]
		public SVsServiceProvider VsServiceProvider { get; set; }
		
		private IVsShell TryGetVsShell() {
			SVsServiceProvider serviceProvider = VsServiceProvider;
			return serviceProvider != null ? serviceProvider.GetService(typeof(IVsShell)) as IVsShell : null;
		}

		/// <summary>
		/// Gets the EnhancedTooltip QuickInfoSourceProvider corresponding to the correct ReSharper version if available.
		/// This way, this package works for any future version of R# without any modification.
		/// </summary>
		private Type TryGetProviderType() {
			Version reSharperVersion = TryGetVsShell().TryGetReSharperVersion();
			if (reSharperVersion == null)
				return null;

			return Type.GetType(ProviderTypeNameWithoutVersion + reSharperVersion.ToString(2), false);
		}

		private void Uninstall() {
			var vsExtensionManager = VsServiceProvider.GetService(typeof(SVsExtensionManager)) as IVsExtensionManager;
			if (vsExtensionManager == null)
				return;
			
			IInstalledExtension thisExtension;
			if (vsExtensionManager.TryGetInstalledExtension("GammaJul.ReSharper.EnhancedTooltip.VsIntegration", out thisExtension))
				vsExtensionManager.Uninstall(thisExtension);
		}

		private IQuickInfoSourceProvider LoadProvider() {
			Type providerType = TryGetProviderType();
			if (providerType == null) {
				// No EnhancedTooltip or No ReSharper?
				// we don't have a reason to exist anymore, as this means EnhancedTooltip or ReSharper has been uninstalled but we didn't.
				Uninstall();
				return null;
			}
			
			return Activator.CreateInstance(providerType, VsEditorAdaptersFactoryService) as IQuickInfoSourceProvider;
		}

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
			IQuickInfoSourceProvider provider = _lazyProvider.Value;
			return provider != null ? provider.TryCreateQuickInfoSource(textBuffer) : null;
		}

		public QuickInfoSourceProviderProxy() {
			 _lazyProvider = new Lazy<IQuickInfoSourceProvider>(LoadProvider, true);
		}

	}

}
