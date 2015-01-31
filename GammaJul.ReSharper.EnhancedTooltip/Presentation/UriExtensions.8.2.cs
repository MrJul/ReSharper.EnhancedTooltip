using System;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.UI.Extensions;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static class UriExtensions {

		public static Uri MakeComponentUri([NotNull] this Assembly assembly, [NotNull] string path) {
			return UriHelpers.MakeUriToExecutingAssemplyResource(path, assembly);
		}

	}

}