using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	public static class ExpressionTypeExtensions {

		public static bool HasSameFullNameAs([CanBeNull] this IExpressionType source, [CanBeNull] IExpressionType other) {
			return source != null
				&& other != null
				&& source.GetLongPresentableName(CSharpLanguage.Instance) == other.GetLongPresentableName(CSharpLanguage.Instance);
		}

	}

}