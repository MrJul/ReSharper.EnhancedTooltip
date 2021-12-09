namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class AttributeUsageContent {

		public string ValidOn { get; }

		public string AllowMultiple { get; }

		public string Inherited { get; }

		private static string ToYesNoString(bool value)
			=> value ? "Yes" : "No";

		public AttributeUsageContent(string validOn, bool allowMultiple, bool inherited) {
			ValidOn = validOn;
			AllowMultiple = ToYesNoString(allowMultiple);
			Inherited = ToYesNoString(inherited);
		}

	}

}