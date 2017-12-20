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
		public static object[] RetrieveVsSquiggleContents([CanBeNull] this IQuickInfoSession session) {
			var properties = session?.Properties;
			if (properties != null) {
				if (properties.TryGetProperty(_squiggleContentsPropertyKey, out object[] squiggleContents)) {
					properties.RemoveProperty(_squiggleContentsPropertyKey);
					return squiggleContents ?? EmptyArray<object>.Instance;
				}
			}
			return EmptyArray<object>.Instance;
		}

	}

}