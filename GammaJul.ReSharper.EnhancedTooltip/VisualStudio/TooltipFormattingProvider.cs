using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Util.Lazy;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[ShellComponent]
	public class TooltipFormattingProvider {

		private readonly Lazy<Optional<IClassificationFormatMapService>> _lazyFormatMapService;
		private readonly Lazy<IClassificationFormatMap> _lazyFormatMap;

		[NotNull]
		public TextFormattingRunProperties GetTooltipFormatting() {
			IClassificationFormatMap formatMap = _lazyFormatMap.Value;
			return formatMap != null ? formatMap.DefaultTextProperties : TextFormattingRunProperties.CreateTextFormattingRunProperties();
		}

		[CanBeNull]
		private IClassificationFormatMap FindFormatMap() {
			IClassificationFormatMapService formatMapService = _lazyFormatMapService.Value.CanBeNull;
			return formatMapService != null ? formatMapService.GetClassificationFormatMap("tooltip") : null;
		}

		public TooltipFormattingProvider([NotNull] Lazy<Optional<IClassificationFormatMapService>> lazyFormatMapService) {
			_lazyFormatMapService = lazyFormatMapService;
			_lazyFormatMap = Lazy.Of(FindFormatMap);
		}

	}

}