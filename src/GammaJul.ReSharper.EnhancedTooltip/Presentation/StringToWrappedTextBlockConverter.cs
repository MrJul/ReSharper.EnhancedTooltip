using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class StringToWrappedTextBlockConverter : IValueConverter {
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			switch (value) {
				case null:
					return null;
				case string stringValue:
					return new TextBlock { Text = stringValue, TextWrapping = TextWrapping.Wrap };
				default:
					return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			switch (value) {
				case null:
					return null;
				case TextBlock textBlock:
					return textBlock.Text;
				default:
					return value;
			}
		}

	}

}