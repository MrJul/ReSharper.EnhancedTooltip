using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Language.Intellisense;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	internal static class QuickInfoSessionExtensions {

		[NotNull] private static readonly object _squiggleContentsPropertyKey = new object();

		public static void StoreVsSquiggleContents([NotNull] this IQuickInfoSession session, [CanBeNull] HashSet<object> squiggleContents) {
			session.Properties.AddProperty(_squiggleContentsPropertyKey, squiggleContents);
		}

		[NotNull]
		public static HashSet<object> RetrieveVsSquiggleContents([NotNull] this IQuickInfoSession session) {
			HashSet<object> squiggleContents;
			return session.Properties.TryGetProperty(_squiggleContentsPropertyKey, out squiggleContents)
				? (squiggleContents ?? new HashSet<object>())
				: new HashSet<object>();
		}

	}

}