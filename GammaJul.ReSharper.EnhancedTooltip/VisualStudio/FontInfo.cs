using System.Windows.Media;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public struct FontInfo {

		private readonly Typeface _typeface;
		private readonly double? _size;
		private readonly Brush _foreground;

		[CanBeNull]
		public Typeface Typeface {
			get { return _typeface; }
		}

		public double? Size {
			get { return _size; }
		}

		[CanBeNull]
		public Brush Foreground {
			get { return _foreground; }
		}

		public FontInfo(Typeface typeface, double? size, Brush foreground) {
			_typeface = typeface;
			_size = size;
			_foreground = foreground;
		}

	}

}