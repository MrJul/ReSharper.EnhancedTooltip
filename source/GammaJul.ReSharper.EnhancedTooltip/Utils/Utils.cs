using System;

namespace GammaJul.ReSharper.EnhancedTooltip.Utils {

  using System.Linq;
  using System.Reflection;

  using GammaJul.ReSharper.EnhancedTooltip.ExternalHighlightings;

  using JetBrains.Reflection;
  using JetBrains.ReSharper.Psi;

  public static class Utils {

    public static Object? GetDynamicFieldOrPropertySafe(this Object obj, String memberName) {
      try {
        return obj.GetDynamicFieldOrProperty(memberName);
      } catch {
        return null;
      }
    }

    public static Type? GetExternalHighlightingType(this Type sourceType) {
      var typeAssembly = Assembly.GetExecutingAssembly();
      return typeAssembly.GetTypes().SingleOrDefault(t => t.IsInterface && t.Name == "I" + sourceType.Name && t.GetInterfaces().Contains(typeof(IExternalHighlighting)));
    }
  }
}
