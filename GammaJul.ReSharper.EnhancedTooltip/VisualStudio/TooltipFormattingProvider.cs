using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Util.Lazy;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[ShellComponent]
	public class TooltipFormattingProvider {

		[NotNull] [ItemCanBeNull] private readonly Lazy<Optional<IClassificationFormatMapService>> _lazyFormatMapService;
		[NotNull] [ItemCanBeNull] private readonly Lazy<IClassificationFormatMap> _lazyFormatMap;

		[NotNull]
		public TextFormattingRunProperties GetTooltipFormatting()
			=> _lazyFormatMap.Value?.DefaultTextProperties ?? TextFormattingRunProperties.CreateTextFormattingRunProperties();

		[CanBeNull]
		private IClassificationFormatMap FindFormatMap()
			=> _lazyFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("tooltip");

		public TooltipFormattingProvider([NotNull] Lazy<Optional<IClassificationFormatMapService>> lazyFormatMapService) {
			_lazyFormatMapService = lazyFormatMapService;
			_lazyFormatMap = Lazy.Of(FindFormatMap);
		}

	}

}