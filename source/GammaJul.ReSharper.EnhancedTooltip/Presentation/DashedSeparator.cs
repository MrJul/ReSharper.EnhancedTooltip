using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// A dashed separator that fills the width of its parent.
	/// </summary>
	/// <remarks>
	/// <see cref="Line"/> is a pain to work with its absolute coordinates.
	/// <see cref="Path"/> stretches inappropriately when there's a <see cref="FrameworkElement.MaxWidth"/> on its parent.
	/// </remarks>
	public sealed class DashedSeparator : Shape {

		static DashedSeparator() {
			StretchProperty.OverrideMetadata(typeof(DashedSeparator), new FrameworkPropertyMetadata(Stretch.Fill));
			StrokeProperty.OverrideMetadata(typeof(DashedSeparator), new FrameworkPropertyMetadata(OnPenChanged));
			StrokeThicknessProperty.OverrideMetadata(typeof(DashedSeparator), new FrameworkPropertyMetadata(OnPenChanged));
		}
		
		private double _width;
		private Rect _rect = Rect.Empty;
		private Pen? _pen;

		public override Geometry RenderedGeometry
			=> DefiningGeometry;

		public override Transform GeometryTransform
			=> Transform.Identity;

		protected override Size MeasureOverride(Size constraint)
			=> new(0, StrokeThickness);

		protected override Size ArrangeOverride(Size finalSize) {
			_width = Math.Max(0.0, finalSize.Width);

			double halfStrokeThickness = StrokeThickness / 2.0;
			_rect = new Rect(new Point(0.0, halfStrokeThickness), new Point(_width, halfStrokeThickness));

			return finalSize;
		}

		protected override Geometry DefiningGeometry
			=> new LineGeometry(_rect.TopLeft, _rect.BottomRight);
		
		protected override void OnRender(DrawingContext drawingContext) {
			drawingContext.DrawLine(Pen, _rect.TopLeft, _rect.BottomRight);
		}

		private Pen Pen
			=> _pen ??= CreatePen();

		private Pen CreatePen() {
			var pen = new Pen(Stroke, StrokeThickness) {
				DashStyle = new DashStyle(new[] { 3.0, 3.0 }, 0.0)
			};
			pen.Freeze();
			return pen;
		}

		private static void OnPenChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e) {
			if (d is DashedSeparator dashedSeparator)
				dashedSeparator._pen = null;
		}

	}

}