using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class StringToWrappedTextBlockConverter : IValueConverter {
		
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value switch {
				null => null,
				string stringValue => new TextBlock { Text = stringValue, TextWrapping = TextWrapping.Wrap },
				_ => value
			};

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> value switch {
				null => null,
				TextBlock textBlock => textBlock.Text,
				_ => value
			};

	}

}