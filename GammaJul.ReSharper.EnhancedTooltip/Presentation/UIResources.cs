using System.Windows;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Util.Lazy;
#if RS90
using JetBrains.ReSharper.Resources.Shell;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	[ShellComponent]
	internal sealed partial class UIResources {

		[NotNull]
		public static UIResources Instance {
			get { return Shell.Instance.GetComponent<UIResources>(); }
		}

		private readonly Lazy<ResourceDictionary> _lazyResourceDictionary = Lazy.Of(LoadResourceDictionary);
		
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