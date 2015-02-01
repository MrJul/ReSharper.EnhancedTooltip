using System.Windows;
using JetBrains.Annotations;
using JetBrains.UI.Extensions;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal sealed partial class UIResources {
		
		[NotNull]
		private static ResourceDictionary LoadResourceDictionary() {
			return new ResourceDictionary {
				Source = UriHelpers.MakeUriToExecutingAssemplyResource("Presentation/UIResources.8.2.xaml", typeof(UIResources).Assembly)
			};
		}

	}

}