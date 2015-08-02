using JetBrains.Annotations;
using JetBrains.Util;
using Microsoft.VisualStudio.Language.Intellisense;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	internal static class QuickInfoSessionExtensions {

		[NotNull] private static readonly object _squiggleContentsPropertyKey = new object();

		public static void StoreVsSquiggleContents([NotNull] this IQuickInfoSession session, [CanBeNull] object[] squiggleContents) {
			session.Properties[_squiggleContentsPropertyKey] = squiggleContents;
		}

		[NotNull]
		public static object[] RetrieveVsSquiggleContents([NotNull] this IQuickInfoSession session) {
			object[] squiggleContents;
			if (session.Properties.TryGetProperty(_squiggleContentsPropertyKey, out squiggleContents)) {
				session.Properties.RemoveProperty(_squiggleContentsPropertyKey);
				return squiggleContents ?? EmptyArray<object>.Instance;
			}
			return EmptyArray<object>.Instance;
		}

	}

}