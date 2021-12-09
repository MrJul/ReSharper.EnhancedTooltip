using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Application;
using JetBrains.Application.UI.Extensions;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[ShellComponent]
	internal sealed class UIResources {

		public static UIResources Instance
			=> Shell.Instance.GetComponent<UIResources>();

		private readonly Lazy<ResourceDictionary> _lazyResourceDictionary = Lazy.Of(LoadResourceDictionary, true);
		
		public Style HeaderedContentControlStyle
			=> (Style) _lazyResourceDictionary.Value["HeaderedContentControlStyle"];

		public Style QuickInfoListBoxStyle
			=> (Style) _lazyResourceDictionary.Value["QuickInfoListBoxStyle"];

		public DataTemplate QuickInfoItemDataTemplate
			=> (DataTemplate) _lazyResourceDictionary.Value["QuickInfoItemDataTemplate"];

		public DataTemplate LegacyQuickInfoItemDataTemplate
			=> (DataTemplate) _lazyResourceDictionary.Value["LegacyQuickInfoItemDataTemplate"];

		public ControlTemplate QuickInfoRootControlTemplate
			=> (ControlTemplate) _lazyResourceDictionary.Value["QuickInfoRootControlTemplate"];

		private static ResourceDictionary LoadResourceDictionary()
			=> new ResourceDictionary {
				Source = UriHelpers.MakeUri("Presentation/UIResources.xaml", typeof(UIResources).Assembly)
			};

	}

}