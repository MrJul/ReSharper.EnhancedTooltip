using JetBrains.Application.Settings;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how tooltips are shown for identifiers.")]
	public class IdentifierTooltipSettings {

		[SettingsEntry(true, "Display an icon for identifiers.")]
		public bool ShowIcon { get; set; }

		[SettingsEntry(true, "Display the identifier kind (method, property, etc.).")]
		public bool ShowKind { get; set; }

		[SettingsEntry(true, "Display whether the member is obsolete.")]
		public bool ShowObsolete { get; set; }

		[SettingsEntry(true, "Display documented exceptions that can be thrown.")]
		public bool ShowExceptions { get; set; }

		[SettingsEntry(true, "Use type keywords when possible (eg. int instead of Int32).")]
		public bool UseTypeKeywords { get; set; }

		[SettingsEntry(true, "Display [CanBeNull] and [NotNull] for identifiers.")]
		public bool ShowIdentifierNullness { get; set; }

		[SettingsEntry(true, "Display [CanBeNull] and [NotNull] for parameters inside calls.")]
		public bool ShowParametersNullness { get; set; }

	}

}