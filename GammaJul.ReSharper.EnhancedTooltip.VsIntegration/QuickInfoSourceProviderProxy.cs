using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace GammaJul.ReSharper.EnhancedTooltip.VsIntegration {

	/// <summary>
	/// This class delegates to the real provider in the main extension assembly if it's available.
	/// This is in aseparate assembly without any reference to ReSharper or the main project.
	/// Visual Studio can load it without problems even if Enhanced Tooltip or ReSharper are uninstalled.
	/// </summary>
	[Name("EnhancedTooltipQuickInfo")]
	[Order(After = "ReSharperQuickInfo")]
	[ContentType("text")]
	[Export(typeof(IQuickInfoSourceProvider))]
	public sealed class QuickInfoSourceProviderProxy : IQuickInfoSourceProvider {

		private const string ProviderTypeName = "GammaJul.ReSharper.EnhancedTooltip.VisualStudio.QuickInfoSourceProvider, GammaJul.ReSharper.EnhancedTooltip.8.2";

		private readonly Lazy<IQuickInfoSourceProvider> _lazyProvider;

		[Import]
		public IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService { get; set; }

		private IQuickInfoSourceProvider LoadProvider() {
			Type providerType = Type.GetType(ProviderTypeName, false);
			if (providerType == null)
				return null;
			
			return Activator.CreateInstance(providerType, VsEditorAdaptersFactoryService) as IQuickInfoSourceProvider;
		}

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
			IQuickInfoSourceProvider provider = _lazyProvider.Value;
			return provider != null ? provider.TryCreateQuickInfoSource(textBuffer) : null;
		}

		public QuickInfoSourceProviderProxy() {
			 _lazyProvider = new Lazy<IQuickInfoSourceProvider>(LoadProvider);
		}

	}

}
