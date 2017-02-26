using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class FontSizeScaleConverter : IValueConverter {

		public double Scale { get; set; } = 1.0;
		
		[CanBeNull]
		private static object ScaleDouble([CanBeNull] object value, double scale) {
			if (!(value is double))
				return DependencyProperty.UnsetValue;
			return ((double) value) * scale;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> ScaleDouble(value, Scale);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> ScaleDouble(value, 1.0 / Scale);

	}

}