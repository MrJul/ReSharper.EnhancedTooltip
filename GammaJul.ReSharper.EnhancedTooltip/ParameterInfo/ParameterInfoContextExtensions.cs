using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	public static class ParameterInfoContextExtensions {

		[Pure]
		[ContractAnnotation("context:null => false")]
		public static bool CanEnhance(
			[CanBeNull] this IParameterInfoContext context,
			[NotNull] IContextBoundSettingsStore settings)
			=> context != null && settings.GetValue((ParameterInfoSettings s) => s.Enabled);

	}

}