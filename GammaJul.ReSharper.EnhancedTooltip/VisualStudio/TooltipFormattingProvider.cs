using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Util.Lazy;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[ShellComponent]
	public class TooltipFormattingProvider {

		[NotNull] [ItemCanBeNull] private readonly Lazy<IClassificationFormatMap> _lazyTooltipFormatMap;
		[NotNull] [ItemCanBeNull] private readonly Lazy<IClassificationFormatMap> _lazyTextFormatMap;
		[NotNull] [ItemCanBeNull] private readonly Lazy<ResourceDictionary> _lazyTextViewBackgroundResources;

		[NotNull]
		public TextFormattingRunProperties GetTooltipFormatting()
			=> _lazyTooltipFormatMap.Value?.DefaultTextProperties ?? TextFormattingRunProperties.CreateTextFormattingRunProperties();

		[CanBeNull]
		public Brush TryGetBackgroundBrush()
			=> _lazyTextViewBackgroundResources.Value?["Background"] as Brush;

		[CanBeNull]
		public Brush TryGetForegroundBrush() {
			TextFormattingRunProperties textProperties = _lazyTextFormatMap.Value?.DefaultTextProperties;
			return textProperties == null || textProperties.ForegroundBrushEmpty ? null : textProperties.ForegroundBrush;
		}

		public TooltipFormattingProvider(
			[NotNull] Lazy<Optional<IClassificationFormatMapService>> lazyFormatMapService,
			[NotNull] Lazy<Optional<IEditorFormatMapService>> lazyEditorFormatMapService) {
			_lazyTooltipFormatMap = Lazy.Of(
				() => lazyFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("tooltip"));
			_lazyTextFormatMap = Lazy.Of(
				() => lazyFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("text"));
			_lazyTextViewBackgroundResources = Lazy.Of(
				() => lazyEditorFormatMapService.Value.CanBeNull?.GetEditorFormatMap("text").GetProperties("TextView Background"));
		}

	}

}