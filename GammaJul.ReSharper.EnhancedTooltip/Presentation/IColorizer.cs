using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal interface IColorizer {

		void AppendDeclaredElement([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution);

	}

}