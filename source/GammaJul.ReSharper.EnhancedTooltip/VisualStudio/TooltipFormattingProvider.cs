using System;
using System.Windows;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Application.UI.Components.Theming;
using JetBrains.Util;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	[ShellComponent]
	public class TooltipFormattingProvider {

		private readonly Lazy<IClassificationFormatMap?> _lazyTooltipFormatMap;
		private readonly Lazy<IClassificationFormatMap?> _lazyTextFormatMap;
		private readonly Lazy<ResourceDictionary?> _lazyTextViewBackgroundResources;

		public TextFormattingRunProperties GetTooltipFormatting()
			=> _lazyTooltipFormatMap.Value?.DefaultTextProperties ?? TextFormattingRunProperties.CreateTextFormattingRunProperties();

		public Brush? TryGetBackground(TooltipColorSource colorSource) {
			if (colorSource == TooltipColorSource.EnvironmentSettings)
				return GetAppBrush(ThemeColor.TooltipBackground.BrushKey);

			return _lazyTextViewBackgroundResources.Value?["Background"] as Brush;
		}

		public Brush? TryGetForeground(TooltipColorSource colorSource) {
			if (colorSource == TooltipColorSource.EnvironmentSettings)
				return GetAppBrush(ThemeColor.TooltipForeground.BrushKey);

			return _lazyTextFormatMap.Value?.DefaultTextProperties is { ForegroundBrushEmpty: false } textProperties 
				? textProperties.ForegroundBrush 
				: null;
		}

		public Brush? TryGetBorderBrush()
			=> GetAppBrush(ThemeColor.TooltipBorder.BrushKey);

		[Pure]
		private static Brush? GetAppBrush(Object brushKey)
			=> Application.Current.Resources[brushKey] as Brush;

		public TooltipFormattingProvider(
			Lazy<Optional<IClassificationFormatMapService>> lazyFormatMapService,
			Lazy<Optional<IEditorFormatMapService>> lazyEditorFormatMapService) {
			_lazyTooltipFormatMap = Lazy.Of(
				() => lazyFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("tooltip"),
				true);
			_lazyTextFormatMap = Lazy.Of(
				() => lazyFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("text"),
				true);
			_lazyTextViewBackgroundResources = Lazy.Of(
				() => lazyEditorFormatMapService.Value.CanBeNull?.GetEditorFormatMap("text").GetProperties("TextView Background"),
				true);
		}

	}

}