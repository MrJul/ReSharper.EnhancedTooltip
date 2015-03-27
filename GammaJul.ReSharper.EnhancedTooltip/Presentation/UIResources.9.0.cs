using System.Windows;
using JetBrains.Annotations;
using JetBrains.UI.Extensions;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal sealed partial class UIResources {
		
		[NotNull]
		private static ResourceDictionary LoadResourceDictionary() {
			return new ResourceDictionary {
				Source = UriHelpers.MakeUri("Presentation/UIResources.9.0.generated.xaml", typeof(UIResources).Assembly)
			};
		}

	}

}