using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	internal static class CandidateExtensions {
		
		public static RichText GetSignature(this ICandidate candidate, string[] namedArguments, AnnotationsDisplayKind showAnnotations,
			out TextRange[] parameterRanges, out int[] mapToOriginalOrder, out ExtensionMethodInfo extensionMethodInfo) {
			return candidate.GetSignature(namedArguments, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
		}

	}

}