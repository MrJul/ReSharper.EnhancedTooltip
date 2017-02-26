using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public sealed class StringToWrappedTextBlockConverter : IValueConverter {
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return null;

			var stringValue = value as string;
			if (stringValue == null)
				return value;

			return new TextBlock {
				Text = stringValue,
				TextWrapping = TextWrapping.Wrap
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return null;

			var textBlock = value as TextBlock;
			if (textBlock == null)
				return value;

			return textBlock.Text;
		}

	}

}