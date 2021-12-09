using System;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	internal sealed class EnumValue {

		[UsedImplicitly]
		public Enum Value { get; }

		[UsedImplicitly]
		public string? Description { get; }

		public EnumValue(Enum value, string? description) {
			Value = value;
			Description = description;
		}

	}

}