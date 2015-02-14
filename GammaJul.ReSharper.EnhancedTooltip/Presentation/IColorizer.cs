using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal interface IColorizer {

		[NotNull]
		PresentedInfo AppendDeclaredElement([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, [NotNull] PresenterOptions options);

	}

}