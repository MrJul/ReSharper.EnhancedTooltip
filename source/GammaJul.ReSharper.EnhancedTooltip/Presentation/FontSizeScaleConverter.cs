using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class FontSizeScaleConverter : IValueConverter {

		public double Scale { get; set; } = 1.0;
		
		private static object ScaleDouble(object? value, double scale)
			=> value is double d ? d * scale : DependencyProperty.UnsetValue;

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> ScaleDouble(value, Scale);

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> ScaleDouble(value, 1.0 / Scale);

	}

}