using JetBrains.Util;
using Microsoft.VisualStudio.Language.Intellisense;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	internal static class QuickInfoSessionExtensions {

		private static readonly object _squiggleContentsPropertyKey = new();

		public static void StoreVsSquiggleContents(this IQuickInfoSession session, object?[]? squiggleContents) {
			session.Properties[_squiggleContentsPropertyKey] = squiggleContents;
		}

		public static object?[] RetrieveVsSquiggleContents(this IQuickInfoSession? session) {
			if (session?.Properties is { } properties && properties.TryGetProperty(_squiggleContentsPropertyKey, out object?[]? squiggleContents)) {
				properties.RemoveProperty(_squiggleContentsPropertyKey);
				return squiggleContents ?? EmptyArray<object?>.Instance;
			}
			return EmptyArray<object?>.Instance;
		}

	}

}