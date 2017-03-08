using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Lookup;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public static class AnnotationsDisplayKindExtensions {

		[Pure]
		public static AttributesDisplayKind ToAttributesDisplayKind(this AnnotationsDisplayKind annotationsDisplayKind) {
			switch (annotationsDisplayKind) {
				case AnnotationsDisplayKind.None:
					return AttributesDisplayKind.Never;
				case AnnotationsDisplayKind.Nullness:
					return AttributesDisplayKind.NullnessAnnotations;
				case AnnotationsDisplayKind.All:
					return AttributesDisplayKind.AllAnnotations;
				default:
					return AttributesDisplayKind.Never;
			}
		}

	}

}