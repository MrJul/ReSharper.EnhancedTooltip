using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	internal interface IInvocationCandidateCountProvider {

		int? TryGetInvocationCandidateCount(IReference reference);

	}

}