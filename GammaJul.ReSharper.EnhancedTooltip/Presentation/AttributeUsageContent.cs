using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class AttributeUsageContent {

		[NotNull]
		public string ValidOn { get; }

		[NotNull]
		public string AllowMultiple { get; }

		[NotNull]
		public string Inherited { get; }

		[NotNull]
		private static string ToYesNoString(bool value)
			=> value ? "Yes" : "No";

		public AttributeUsageContent([NotNull] string validOn, bool allowMultiple, bool inherited) {
			ValidOn = validOn;
			AllowMultiple = ToYesNoString(allowMultiple);
			Inherited = ToYesNoString(inherited);
		}

	}

}