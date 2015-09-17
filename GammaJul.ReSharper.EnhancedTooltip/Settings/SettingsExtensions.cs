using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public static class SettingsExtensions {

		[NotNull]
		public static IContextBoundSettingsStore GetSettings([NotNull] this IDocument document)
			=> Shell.Instance
				.GetComponent<ISettingsStore>()
				.BindToContextTransient(ContextRange.Smart(document.ToDataContext()));

		[CanBeNull]
		public static IContextBoundSettingsStore TryGetSettings([CanBeNull] this IDocument document)
			=> document != null && Shell.HasInstance ? document.GetSettings() : null;

	}

}