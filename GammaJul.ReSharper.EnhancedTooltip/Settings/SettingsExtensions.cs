using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
#if RS90
using JetBrains.ReSharper.Resources.Shell;
#elif RS82
using JetBrains.Application;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public static class SettingsExtensions {

		[NotNull]
		public static IContextBoundSettingsStore GetSettings([NotNull] this IDocument document) {
			var settingsStore = Shell.Instance.GetComponent<ISettingsStore>();
			return settingsStore.BindToContextTransient(ContextRange.Smart(document.ToDataContext()));
		}

	}

}