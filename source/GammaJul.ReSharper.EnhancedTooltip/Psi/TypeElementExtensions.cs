using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	public static class TypeElementExtensions {

		[Pure]
		public static bool IsInSystemLikeNamespace(this ITypeElement typeElement)
			=> typeElement.GetClrName().NamespaceNames.FirstOrDefault() == "System";

	}

}