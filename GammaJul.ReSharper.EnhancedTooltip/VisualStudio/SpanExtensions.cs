
using System;
using Microsoft.VisualStudio.Text;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public static class SpanExtensions {

		public static Span Union(this Span x, Span? y) {
			if (y == null)
				return x;

			return Span.FromBounds(
				Math.Min(x.Start, y.Value.Start),
				Math.Max(x.End, y.Value.End));
		}

	}

}