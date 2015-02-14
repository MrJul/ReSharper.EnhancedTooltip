using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Resolve;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static class CSharpColorizerExtensions {

		public static void AppendCandidates([NotNull] this CSharpColorizer colorizer, [CanBeNull] IReference reference) {
			if (reference == null)
				return;

			IResolveResult resolveResult = reference.Resolve().Result;
			var liftedResolveResult = resolveResult as LiftedResolveResult;

			// ReSharper disable once CoVariantArrayConversion
			IList<IDeclaredElement> candidates = liftedResolveResult != null ? liftedResolveResult.LiftedCandidates : resolveResult.Candidates;
			
			IList<ISubstitution> substitutions = resolveResult.CandidateSubstitutions;

			for (int i = 0; i < candidates.Count; ++i) {
				colorizer.AppendPlainText("\r\n  ");
				colorizer.AppendDeclaredElement(candidates[i].EliminateDelegateInvokeMethod(), substitutions[i]);
			}
		}

	}

}