using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how tooltips are shown for identifiers.")]
	public class IdentifierTooltipSettings {

		[SettingsEntry(true, "Enhance identifier tooltips.")]
		public bool Enabled { get; set; }

		[SettingsEntry(true, "Display an icon for identifiers.")]
		public bool ShowIcon { get; set; }

		[SettingsEntry(true, "Display the identifier kind (method, property, etc.).")]
		public bool ShowKind { get; set; }

		[SettingsEntry(true, "Display whether the member is obsolete.")]
		public bool ShowObsolete { get; set; }

		[SettingsEntry(true, "Display documented exceptions that can be thrown.")]
		public bool ShowExceptions { get; set; }

		[SettingsEntry(true, "Use type keywords (eg. int instead of Int32).")]
		public bool UseTypeKeywords { get; set; }

		[SettingsEntry(true, "Display annotations for identifiers.")]
		public AnnotationsDisplayKind ShowIdentifierAnnotations { get; set; }

		[SettingsEntry(true, "Display annotations for parameters inside calls.")]
		public AnnotationsDisplayKind ShowParametersAnnotations { get; set; }

	}

}