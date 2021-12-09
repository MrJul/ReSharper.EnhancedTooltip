using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class DescriptionSeparatorWidthConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value is double d ? d + 16.0 : DependencyProperty.UnsetValue;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> DependencyProperty.UnsetValue;

	}

}