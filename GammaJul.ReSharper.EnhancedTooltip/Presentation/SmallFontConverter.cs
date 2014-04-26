using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class SmallFontConverter : IValueConverter {

		public static readonly SmallFontConverter Instance = new SmallFontConverter();

		private const double Scale = 0.9;

		[CanBeNull]
		private static object ScaleDouble([CanBeNull] object value, double scale) {
			if (!(value is double))
				return DependencyProperty.UnsetValue;
			return ((double) value) * scale;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ScaleDouble(value, Scale);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ScaleDouble(value, 1.0 / Scale);
		}

		private SmallFontConverter() {
		}

	}

}