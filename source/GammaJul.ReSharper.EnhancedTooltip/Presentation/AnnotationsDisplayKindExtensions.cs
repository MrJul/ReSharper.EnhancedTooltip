using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Lookup;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public static class AnnotationsDisplayKindExtensions {

		[Pure]
		public static AttributesDisplayKind ToAttributesDisplayKind(this AnnotationsDisplayKind annotationsDisplayKind)
			=> annotationsDisplayKind switch {
				AnnotationsDisplayKind.None => AttributesDisplayKind.Never,
				AnnotationsDisplayKind.Nullness => AttributesDisplayKind.NullnessAnnotations,
				AnnotationsDisplayKind.All => AttributesDisplayKind.AllAnnotations,
				_ => AttributesDisplayKind.Never
			};

	}

}