using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[SettingsKey(typeof(EnhancedTooltipSettingsRoot), "Settings determining how tooltips are shown for identifiers")]
	public class IdentifierTooltipSettings {

		[SettingsEntry(true, "Enhance identifier tooltips")]
		public bool Enabled { get; set; }

		[SettingsEntry(true, "Display an icon for identifiers")]
		public bool ShowIcon { get; set; }

		[SettingsEntry(false, "Display access rights (private/protected/internal/public)")]
		public bool ShowAccessRights { get; set; }

		[SettingsEntry(true, "Display the identifier kind (method/property/etc.)")]
		public bool ShowKind { get; set; }
		
		[SettingsEntry(true, "Display property accessors (get/set)")]
		public bool ShowAccessors { get; set; }

		[SettingsEntry(true, "Display member documentation")]
		public bool ShowDocumentation { get; set; }

		[SettingsEntry(true, "Display whether the member is obsolete")]
		public bool ShowObsolete { get; set; }

		[SettingsEntry(false, "Display return value documentation")]
		public bool ShowReturn { get; set; }

		[SettingsEntry(true, "Display remarks")]
		public bool ShowRemarks { get; set; }

		[SettingsEntry(true, "Display documented exceptions")]
		public bool ShowExceptions { get; set; }

		[SettingsEntry(true, "Display overload count where applicable")]
		public bool ShowOverloadCount { get; set; }

		[SettingsEntry(true, "Display extension methods as \"extension\" instead of \"method\"")]
		public bool UseExtensionMethodKind { get; set; }

		[SettingsEntry(true, "Display attribute classes as \"attribute\" instead of \"class\"")]
		public bool UseAttributeClassKind { get; set; }

		[SettingsEntry(false, "Display method modifiers (virtual/override/abstract/sealed/static/unsafe)")]
		public bool UseMethodModifiersInKind { get; set; }

		[SettingsEntry(false, "Display class modifiers (abstract/sealed/static/unsafe)")]
		public bool UseClassModifiersInKind { get; set; }

		[SettingsEntry(false, "Display the role of arguments inside invocations")]
		public bool ShowArgumentsRole { get; set; }

		[SettingsEntry(false, "Display the base type of classes [legacy]")]
		public bool ShowBaseType { get; set; }

		[SettingsEntry(false, "Display the implemented interfaces of types [legacy]")]
		public bool ShowImplementedInterfaces { get; set; }

		[SettingsEntry(true, "Use type keywords (eg. int instead of Int32)")]
		public bool UseTypeKeywords { get; set; }

		[SettingsEntry(true, "Use short nullable form (T? instead of Nullable<T>)")]
		public bool UseShortNullableForm { get; set; }

		[SettingsEntry(true, "Display usage for attribute classes")]
		public bool ShowAttributesUsage { get; set; }

		[SettingsEntry(AnnotationsDisplayKind.Nullness, "Display annotations for identifiers")]
		public AnnotationsDisplayKind ShowIdentifierAnnotations { get; set; }

		[SettingsEntry(AnnotationsDisplayKind.Nullness, "Display annotations for parameters inside method signatures")]
		public AnnotationsDisplayKind ShowParametersAnnotations { get; set; }

		[SettingsEntry(ConstructorReferenceDisplay.ConstructorOnly, "Display constructor references for other types as")]
		public ConstructorReferenceDisplay ConstructorReferenceDisplay { get; set; }

		[SettingsEntry(ConstructorReferenceDisplay.TypeOnly, "Display constructor references for attributes as")]
		public ConstructorReferenceDisplay AttributeConstructorReferenceDisplay { get; set; }

		[SettingsEntry(SolutionCodeNamespaceDisplayKind.Always, "Display namespaces for member from solution code")]
		public SolutionCodeNamespaceDisplayKind SolutionCodeNamespaceDisplayKind { get; set; }

		[SettingsEntry(SolutionCodeNamespaceDisplayKind.Always, "Display namespaces for member from external code")]
		public ExternalCodeNamespaceDisplayKind ExternalCodeNamespaceDisplayKind { get; set; }

		[SettingsEntry(BaseTypeDisplayKind.Never, "Display the base type of classes")]
		public BaseTypeDisplayKind BaseTypeDisplayKind { get; set; }

		[SettingsEntry(ImplementedInterfacesDisplayKind.Never, "Display the implemented interfaces of types")]
		public ImplementedInterfacesDisplayKind ImplementedInterfacesDisplayKind { get; set; }

	}

}