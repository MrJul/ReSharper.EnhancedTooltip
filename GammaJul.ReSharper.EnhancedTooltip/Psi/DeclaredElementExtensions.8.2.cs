using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	internal static partial class DeclaredElementExtensions {

		[Pure]
		[NotNull]
		public static IDeclaredElement EliminateDelegateInvokeMethod([NotNull] this IDeclaredElement declaredElement) {
			return CSharpDeclaredElementUtil.EliminateDelegateInvokeMethod(declaredElement);
		}

	}

}