using System;
using System.Windows;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Platform.VisualStudio.SinceVs11.Shell.Theming;
using JetBrains.Util;
using Microsoft.VisualStudio.Shell;
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
		public Brush TryGetBackground(TooltipColorSource colorSource) {
			if (colorSource == TooltipColorSource.EnvironmentSettings)
				return GetAppBrush(BundledThemeColors.Environment.ToolTipBrushKey);

			return _lazyTextViewBackgroundResources.Value?["Background"] as Brush;
		}

		[CanBeNull]
		public Brush TryGetForeground(TooltipColorSource colorSource) {
			if (colorSource == TooltipColorSource.EnvironmentSettings)
				return GetAppBrush(BundledThemeColors.Environment.ToolTipTextBrushKey);

			TextFormattingRunProperties textProperties = _lazyTextFormatMap.Value?.DefaultTextProperties;
			return textProperties == null || textProperties.ForegroundBrushEmpty ? null : textProperties.ForegroundBrush;
		}

		[CanBeNull]
		public Brush TryGetBorderBrush()
			=> GetAppBrush(BundledThemeColors.Environment.ToolTipBorderBrushKey);

		[CanBeNull]
		[Pure]
		private static Brush GetAppBrush([NotNull] ThemeResourceKey brushKey)
			=> Application.Current.Resources[brushKey] as Brush;

		public TooltipFormattingProvider(
			[NotNull] Lazy<Optional<IClassificationFormatMapService>> lazyFormatMapService,
			[NotNull] Lazy<Optional<IEditorFormatMapService>> lazyEditorFormatMapService) {
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