using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal sealed class DescriptionSeparatorWidthConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is double))
				return DependencyProperty.UnsetValue;

			return (double) value + 16;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> DependencyProperty.UnsetValue;

	}

}