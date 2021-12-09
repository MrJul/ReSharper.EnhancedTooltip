using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public static class SpanExtensions {

		[Pure]
		public static Span Union(this Span x, Span? y) {
			if (y is not { } other)
				return x;

			return Span.FromBounds(
				Math.Min(x.Start, other.Start),
				Math.Max(x.End, other.End));
		}

	}

}