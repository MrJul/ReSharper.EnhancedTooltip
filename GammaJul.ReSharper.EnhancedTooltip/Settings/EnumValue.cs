using System;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	internal sealed class EnumValue {

		[NotNull] private readonly Enum _value;
		[CanBeNull] private readonly string _description;

		[NotNull]
		public Enum Value {
			get { return _value; }
		}

		[CanBeNull]
		public string Description {
			get { return _description; }
		}

		public EnumValue([NotNull] Enum value, [CanBeNull] string description) {
			_value = value;
			_description = description;
		}

	}

}