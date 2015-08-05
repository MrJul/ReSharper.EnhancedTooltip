using System.Windows;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.UI.Extensions;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[ShellComponent]
	internal sealed class UIResources {

		[NotNull]
		public static UIResources Instance
			=> Shell.Instance.GetComponent<UIResources>();

		[NotNull] [ItemNotNull] private readonly Lazy<ResourceDictionary> _lazyResourceDictionary = Lazy.Of(LoadResourceDictionary);
		
		[NotNull]
		public Style HeaderedContentControlStyle
			=> (Style) _lazyResourceDictionary.Value["HeaderedContentControlStyle"];

		[NotNull]
		public Style QuickInfoListBoxStyle
			=> (Style) _lazyResourceDictionary.Value["QuickInfoListBoxStyle"];

		[NotNull]
		public Style QuickInfoItemStyle
			=> (Style) _lazyResourceDictionary.Value["QuickInfoItemStyle"];

		[NotNull]
		public DataTemplate QuickInfoItemDataTemplate
			=> (DataTemplate) _lazyResourceDictionary.Value["QuickInfoItemDataTemplate"];

		[NotNull]
		private static ResourceDictionary LoadResourceDictionary()
			=> new ResourceDictionary {
				Source = UriHelpers.MakeUri("Presentation/UIResources.xaml", typeof(UIResources).Assembly)
			};

	}

}