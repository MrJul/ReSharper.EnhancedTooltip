using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using GammaJul.ReSharper.EnhancedTooltip.VisualStudio;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Utils;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.UI.Extensions;
using JetBrains.Util;
using JetBrains.Util.Interop;
using DpiUtil = JetBrains.UI.Utils.DpiUtil;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public static class Styling {

		private sealed class OriginalStyles {
			public Style? Style { get; set; }
			public Style? ItemContainerStyle { get; set; }
			public DataTemplate? ItemTemplate { get; set; }
		}

		public static readonly DependencyProperty ShouldStyleParentListBoxProperty = DependencyProperty.RegisterAttached(
			"ShouldStyleParentListBox",
			typeof(bool),
			typeof(Styling),
			new FrameworkPropertyMetadata(BooleanBoxes.False, OnShouldStyleParentListBoxChanged));

		public static readonly DependencyProperty ItemTemplateBackgroundProperty = DependencyProperty.RegisterAttached(
			"ItemTemplateBackground",
			typeof(Brush),
			typeof(Styling),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static readonly DependencyProperty ItemTemplateBorderBrushProperty = DependencyProperty.RegisterAttached(
			"ItemTemplateBorderBrush",
			typeof(Brush),
			typeof(Styling),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static readonly DependencyProperty DocumentProperty = DependencyProperty.RegisterAttached(
			"Document",
			typeof(WeakReference<IDocument>),
			typeof(Styling),
			new FrameworkPropertyMetadata(null));

		private static readonly DependencyProperty _originalStylesProperty = DependencyProperty.RegisterAttached(
			"OriginalStyles",
			typeof(OriginalStyles),
			typeof(Styling),
			new FrameworkPropertyMetadata(null));

		public static bool GetShouldStyleParentListBox(DependencyObject owner)
			=> (bool) owner.GetValue(ShouldStyleParentListBoxProperty);

		public static void SetShouldStyleParentListBox(DependencyObject owner, bool value)
			=> owner.SetValue(ShouldStyleParentListBoxProperty, value);

		public static Brush? GetItemTemplateBackground(DependencyObject owner)
			=> (Brush?) owner.GetValue(ItemTemplateBackgroundProperty);

		public static void SetItemTemplateBackground(DependencyObject owner, Brush? value)
			=> owner.SetValue(ItemTemplateBackgroundProperty, value);

		public static Brush? GetItemTemplateBorderBrush(DependencyObject owner)
			=> (Brush?) owner.GetValue(ItemTemplateBorderBrushProperty);

		public static void SetItemTemplateBorderBrush(DependencyObject owner, Brush? value)
			=> owner.SetValue(ItemTemplateBorderBrushProperty, value);

		public static WeakReference<IDocument>? GetDocument(DependencyObject owner)
			=> (WeakReference<IDocument>?) owner.GetValue(DocumentProperty);

		public static void SetDocument(DependencyObject owner, WeakReference<IDocument?>? value)
			=> owner.SetValue(DocumentProperty, value);

		private static void OnShouldStyleParentListBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(bool) e.NewValue || d is not FrameworkElement element)
				return;

			element.WhenLoaded(_ => {
				// We're styling the parent VS ListBox included inside the tooltip.
				if (element.FindVisualAncestor(o => o is ItemsControl ic && IsToolTipItemsControl(ic)) is not ItemsControl itemsControl
				|| itemsControl.GetValue(_originalStylesProperty) is not null)
					return;

				IDocument? document = null;
				GetDocument(element)?.TryGetTarget(out document);

				SetItemsControlStyle(itemsControl, document);

				void OnItemsControlUnloaded(object? sender, RoutedEventArgs args) {
					itemsControl.Unloaded -= OnItemsControlUnloaded;
					RestoreOriginalStyles(itemsControl);
				}

				itemsControl.Unloaded += OnItemsControlUnloaded;
			});
		}

		[Pure]
		private static bool IsToolTipItemsControl(ItemsControl itemsControl)
			=> itemsControl.GetType().FullName
				is "System.Windows.Controls.ListBox" // Pre VS 15.6
				or "Microsoft.VisualStudio.Text.AdornmentLibrary.ToolTip.Implementation.WpfToolTipItemsControl"; // VS 15.6

		[Pure]
		private static bool IsToolTipRootControl(UserControl userControl)
			=> userControl.GetType().FullName == "Microsoft.VisualStudio.Text.AdornmentLibrary.ToolTip.Implementation.WpfToolTipControl"; // VS 15.8

		private static void SetItemsControlStyle(ItemsControl itemsControl, IDocument? document) {
			if (itemsControl.GetValue(_originalStylesProperty) is not OriginalStyles) {
				itemsControl.SetValue(_originalStylesProperty, new OriginalStyles {
					Style = itemsControl.Style,
					ItemContainerStyle = itemsControl.ItemContainerStyle,
					ItemTemplate = itemsControl.ItemTemplate
				});
			}

			//itemsControl.Background = new SolidColorBrush(Color.FromArgb(2, 0, 0, 0));
      itemsControl.BorderThickness = new Thickness(1);

			IContextBoundSettingsStore? settings = document.TryGetSettings();

      if (Shell.HasInstance) {
        if (Shell.Instance.TryGetComponent<TooltipFormattingProvider>() is { } tooltipFormattingProvider) {
          var colorSource = settings?.GetValue((DisplaySettings s) => s.TooltipColorSource) ?? TooltipColorSource.TextEditorSettings;

          if (tooltipFormattingProvider.TryGetBackground(colorSource) is { } background)
            itemsControl.Background = background;

          if (tooltipFormattingProvider.TryGetBorderBrush() is { } borderBrush)
            itemsControl.BorderBrush = borderBrush;
        }
      }

			bool isLegacy = itemsControl is ListBox;
			itemsControl.Style = UIResources.Instance.QuickInfoListBoxStyle;
			itemsControl.ItemTemplate = isLegacy ? UIResources.Instance.LegacyQuickInfoItemDataTemplate : UIResources.Instance.QuickInfoItemDataTemplate;
			itemsControl.ItemContainerStyle = CreateItemContainerStyle(settings, isLegacy);
			itemsControl.MaxWidth = ComputeItemsControlMaxWidth(itemsControl,  settings);
			TextOptions.SetTextFormattingMode(itemsControl, GetTextFormattingMode(settings));
			TextOptions.SetTextRenderingMode(itemsControl, TextRenderingMode.Auto);

			if (!isLegacy 
			&& itemsControl.FindVisualAncestor(o => o is UserControl uc && IsToolTipRootControl(uc)) is UserControl rootControl)
				SetRootControlTemplate(rootControl);
		}

		private static void SetRootControlTemplate(UserControl rootControl) {
			ControlTemplate originalTemplate = rootControl.Template;

			void OnRootControlUnloaded(object sender, RoutedEventArgs args) {
				rootControl.Unloaded -= OnRootControlUnloaded;
				rootControl.Template = originalTemplate;
			}

			rootControl.Unloaded += OnRootControlUnloaded;
			rootControl.Template = UIResources.Instance.QuickInfoRootControlTemplate;
		}

		private static Style CreateItemContainerStyle(IContextBoundSettingsStore? settings, bool forListBoxItem) {
			var itemContainerStyle = new Style(forListBoxItem ? typeof(ListBoxItem) : typeof(ContentPresenter));
			itemContainerStyle.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(0.0, 0.0, 0.0, 0.0)));

			if (forListBoxItem) {
				itemContainerStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0.0)));
				itemContainerStyle.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
				itemContainerStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0.0)));
			}

			if (Shell.HasInstance) {
				if (Shell.Instance.TryGetComponent<TooltipFormattingProvider>() is { } tooltipFormattingProvider) {
					var colorSource = settings?.GetValue((DisplaySettings s) => s.TooltipColorSource) ?? TooltipColorSource.TextEditorSettings;

					if (tooltipFormattingProvider.TryGetBackground(colorSource) is { } background)
						itemContainerStyle.Setters.Add(new Setter(ItemTemplateBackgroundProperty, background));

					if (tooltipFormattingProvider.TryGetBorderBrush() is { } borderBrush)
						itemContainerStyle.Setters.Add(new Setter(ItemTemplateBorderBrushProperty, borderBrush));

					if (tooltipFormattingProvider.TryGetForeground(colorSource) is { } foreground)
						itemContainerStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, foreground));
				}
			}

			itemContainerStyle.Seal();
			return itemContainerStyle;
		}

		private static unsafe double ComputeItemsControlMaxWidth(ItemsControl itemsControl, IContextBoundSettingsStore? settings) {
			if (settings is null
			|| !settings.GetValue((DisplaySettings s) => s.LimitTooltipWidth)
			|| PresentationSource.FromVisual(itemsControl) is not HwndSource hwndSource)
				return Double.PositiveInfinity;

			int limitPercent = settings.GetValue((DisplaySettings s) => s.ScreenWidthLimitPercent).Clamp(10, 100);
			IntPtr handle = hwndSource.Handle;
			double dpiFactor = DpiUtil.GetWindowScreenDpiCurrent((void*) handle).DpiX / DpiResolution.DeviceIndependent96DpiValue;
			var screenBounds = ScreenUtil.GetBounds(handle);
			return screenBounds.Width * (limitPercent / 100.0) / dpiFactor;
		}

		private static TextFormattingMode GetTextFormattingMode(IContextBoundSettingsStore? settings)
			=> settings?.GetValue((DisplaySettings s) => s.TextFormattingMode) ?? TextFormattingMode.Ideal;

		private static void RestoreOriginalStyles(ItemsControl listBox) {
			if (listBox.GetValue(_originalStylesProperty) is not OriginalStyles originalStyles)
				return;

			listBox.ClearValue(_originalStylesProperty);

			listBox.Style = originalStyles.Style;
			listBox.ItemContainerStyle = originalStyles.ItemContainerStyle;
			listBox.ItemTemplate = originalStyles.ItemTemplate;
		}

	}

}