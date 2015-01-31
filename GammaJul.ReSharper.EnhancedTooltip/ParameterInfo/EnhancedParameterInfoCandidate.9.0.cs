using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	public partial class EnhancedParameterInfoCandidate {

		public RichText GetSignature(string[] namedArguments, AnnotationsDisplayKind showAnnotations,
			out TextRange[] parameterRanges, out int[] mapToOriginalOrder, out ExtensionMethodInfo extensionMethodInfo) {
			return GetSignatureCore(namedArguments, showAnnotations, out parameterRanges, out mapToOriginalOrder, out extensionMethodInfo);
		}

	}

}