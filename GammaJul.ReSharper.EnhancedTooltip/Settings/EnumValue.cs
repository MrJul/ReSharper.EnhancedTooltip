using System;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	internal sealed class EnumValue {

		[NotNull]
		public Enum Value { get; }

		[CanBeNull]
		public string Description { get; }

		public EnumValue([NotNull] Enum value, [CanBeNull] string description) {
			Value = value;
			Description = description;
		}

	}

}