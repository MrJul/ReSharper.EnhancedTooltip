using System.Windows;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[ShellComponent]
	internal sealed class UIResources {

		[NotNull]
		public static UIResources Instance {
			get { return Shell.Instance.GetComponent<UIResources>(); }
		}

		private readonly Lazy<ResourceDictionary> _lazyResourceDictionary = Lazy.Of(LoadResourceDictionary);

		[NotNull]
		private static ResourceDictionary LoadResourceDictionary() {
			return new ResourceDictionary {
				Source = UriHelpers.MakeUriToExecutingAssemplyResource("Presentation/UIResources.xaml", typeof(UIResources).Assembly)
			};
		}

		[NotNull]
		public Style HeaderedContentControlStyle {
			get { return (Style) _lazyResourceDictionary.Value["HeaderedContentControlStyle"]; }
		}

		[NotNull]
		public Style QuickInfoListBoxStyle {
			get { return (Style) _lazyResourceDictionary.Value["QuickInfoListBoxStyle"]; }
		}

		[NotNull]
		public Style QuickInfoItemStyle {
			get { return (Style) _lazyResourceDictionary.Value["QuickInfoItemStyle"]; }
		}

		[NotNull]
		public DataTemplate QuickInfoItemDataTemplate {
			get { return (DataTemplate) _lazyResourceDictionary.Value["QuickInfoItemDataTemplate"]; }
		}

	}

}