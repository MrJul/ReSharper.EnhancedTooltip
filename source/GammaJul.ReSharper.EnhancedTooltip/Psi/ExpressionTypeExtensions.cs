using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	public static class ExpressionTypeExtensions {

		public static bool HasSameFullNameAs(this IExpressionType? source, IExpressionType? other)
			=> source is not null
				&& other is not null
				&& source.GetLongPresentableName(CSharpLanguage.Instance!) == other.GetLongPresentableName(CSharpLanguage.Instance!);

	}

}