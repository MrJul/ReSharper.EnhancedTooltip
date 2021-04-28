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
			[CanBeNull] public Style Style { get; set; }
			[CanBeNull] public Style ItemContainerStyle { get; set; }
			[CanBeNull] public DataTemplate ItemTemplate { get; set; }
		}

		[NotNull]
		public static readonly DependencyProperty ShouldStyleParentListBoxProperty = DependencyProperty.RegisterAttached(
			"ShouldStyleParentListBox",
			typeof(bool),
			typeof(Styling),
			new FrameworkPropertyMetadata(BooleanBoxes.False, OnShouldStyleParentListBoxChanged));

		[NotNull]
		public static readonly DependencyProperty ItemTemplateBackgroundProperty = DependencyProperty.RegisterAttached(
			"ItemTemplateBackground",
			typeof(Brush),
			typeof(Styling),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		[NotNull]
		public static readonly DependencyProperty ItemTemplateBorderBrushProperty = DependencyProperty.RegisterAttached(
			"ItemTemplateBorderBrush",
			typeof(Brush),
			typeof(Styling),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		[NotNull]
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.RegisterAttached(
			"Document",
			typeof(WeakReference<IDocument>),
			typeof(Styling),
			new FrameworkPropertyMetadata(null));

		[NotNull]
		private static readonly DependencyProperty _originalStylesProperty = DependencyProperty.RegisterAttached(
			"OriginalStyles",
			typeof(OriginalStyles),
			typeof(Styling),
			new FrameworkPropertyMetadata(null));

		public static bool GetShouldStyleParentListBox([NotNull] DependencyObject owner) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			return (bool) owner.GetValue(ShouldStyleParentListBoxProperty);
		}

		public static void SetShouldStyleParentListBox([NotNull] DependencyObject owner, bool value) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			owner.SetValue(ShouldStyleParentListBoxProperty, value);
		}

		[CanBeNull]
		public static Brush GetItemTemplateBackground([NotNull] DependencyObject owner) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			return (Brush) owner.GetValue(ItemTemplateBackgroundProperty);
		}

		public static void SetItemTemplateBackground([NotNull] DependencyObject owner, [CanBeNull] Brush value) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			owner.SetValue(ItemTemplateBackgroundProperty, value);
		}

		[CanBeNull]
		public static Brush GetItemTemplateBorderBrush([NotNull] DependencyObject owner) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			return (Brush) owner.GetValue(ItemTemplateBorderBrushProperty);
		}

		public static void SetItemTemplateBorderBrush([NotNull] DependencyObject owner, [CanBeNull] Brush value) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			owner.SetValue(ItemTemplateBorderBrushProperty, value);
		}

		[CanBeNull]
		public static WeakReference<IDocument> GetDocument([NotNull] DependencyObject owner) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			return (WeakReference<IDocument>) owner.GetValue(DocumentProperty);
		}

		public static void SetDocument([NotNull] DependencyObject owner, [CanBeNull] WeakReference<IDocument> value) {
			if (owner == null)
				throw new ArgumentNullException(nameof(owner));
			owner.SetValue(DocumentProperty, value);
		}

		private static void OnShouldStyleParentListBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(bool) e.NewValue)
				return;

			var element = d as FrameworkElement;

			element?.WhenLoaded(_ => {

				// We're styling the parent VS ListBox included inside the tooltip.
				if (!(element.FindVisualAncestor(o => o is ItemsControl ic && IsToolTipItemsControl(ic)) is ItemsControl itemsControl)
				|| itemsControl.GetValue(_originalStylesProperty) != null)
					return;

				IDocument document = null;
				GetDocument(element)?.TryGetTarget(out document);

				SetItemsControlStyle(itemsControl, document);

				void OnItemsControlUnloaded(object sender, RoutedEventArgs args) {
					itemsControl.Unloaded -= OnItemsControlUnloaded;
					RestoreOriginalStyles(itemsControl);
				}

				itemsControl.Unloaded += OnItemsControlUnloaded;

			});
		}

		[Pure]
		private static bool IsToolTipItemsControl([NotNull] ItemsControl itemsControl) {
			string typeFullName = itemsControl.GetType().FullName;
			return typeFullName == "System.Windows.Controls.ListBox" // Pre VS 15.6
				|| typeFullName == "Microsoft.VisualStudio.Text.AdornmentLibrary.ToolTip.Implementation.WpfToolTipItemsControl"; // VS 15.6
		}

		[Pure]
		private static bool IsToolTipRootControl([NotNull] UserControl userControl)
			=> userControl.GetType().FullName == "Microsoft.VisualStudio.Text.AdornmentLibrary.ToolTip.Implementation.WpfToolTipControl"; // VS 15.8

		private static void SetItemsControlStyle([NotNull] ItemsControl itemsControl, [CanBeNull] IDocument document) {
			if (!(itemsControl.GetValue(_originalStylesProperty) is OriginalStyles)) {
				itemsControl.SetValue(_originalStylesProperty, new OriginalStyles {
					Style = itemsControl.Style,
					ItemContainerStyle = itemsControl.ItemContainerStyle,
					ItemTemplate = itemsControl.ItemTemplate
				});
			}

			IContextBoundSettingsStore settings = document.TryGetSettings();

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

		private static void SetRootControlTemplate([NotNull] UserControl rootControl) {
			ControlTemplate originalTemplate = rootControl.Template;

			void OnRootControlUnloaded(object sender, RoutedEventArgs args) {
				rootControl.Unloaded -= OnRootControlUnloaded;
				rootControl.Template = originalTemplate;
			}

			rootControl.Unloaded += OnRootControlUnloaded;
			rootControl.Template = UIResources.Instance.QuickInfoRootControlTemplate;
		}

		[NotNull]
		private static Style CreateItemContainerStyle([CanBeNull] IContextBoundSettingsStore settings, bool forListBoxItem) {
			var itemContainerStyle = new Style(forListBoxItem ? typeof(ListBoxItem) : typeof(ContentPresenter));
			itemContainerStyle.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(0.0, 0.0, 0.0, 2.0)));

			if (forListBoxItem) {
				itemContainerStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0.0)));
				itemContainerStyle.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
				itemContainerStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0.0)));
			}

			if (Shell.HasInstance) {
				var tooltipFormattingProvider = Shell.Instance.TryGetComponent<TooltipFormattingProvider>();
				if (tooltipFormattingProvider != null) {

					var colorSource = settings?.GetValue((DisplaySettings s) => s.TooltipColorSource) ?? TooltipColorSource.TextEditorSettings;

					var background = tooltipFormattingProvider.TryGetBackground(colorSource);
					if (background != null)
						itemContainerStyle.Setters.Add(new Setter(ItemTemplateBackgroundProperty, background));

					var borderBrush = tooltipFormattingProvider.TryGetBorderBrush();
					if (borderBrush != null)
						itemContainerStyle.Setters.Add(new Setter(ItemTemplateBorderBrushProperty, borderBrush));

					var foreground = tooltipFormattingProvider.TryGetForeground(colorSource);
					if (foreground != null)
						itemContainerStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, foreground));

				}
			}

			itemContainerStyle.Seal();
			return itemContainerStyle;
		}

		private static unsafe double ComputeItemsControlMaxWidth([NotNull] ItemsControl itemsControl, [CanBeNull] IContextBoundSettingsStore settings) {
			if (settings == null
			|| !settings.GetValue((DisplaySettings s) => s.LimitTooltipWidth)
			|| !(PresentationSource.FromVisual(itemsControl) is HwndSource hwndSource))
				return Double.PositiveInfinity;

			int limitPercent = settings.GetValue((DisplaySettings s) => s.ScreenWidthLimitPercent).Clamp(10, 100);
			IntPtr handle = hwndSource.Handle;
			// ReSharper disable once AssignNullToNotNullAttribute
			double dpiFactor = DpiUtil.GetWindowScreenDpiCurrent((void*) handle).DpiX / DpiResolution.DeviceIndependent96DpiValue;
			var screenBounds = ScreenUtil.GetBounds(handle);
			return screenBounds.Width * (limitPercent / 100.0) / dpiFactor;
		}

		private static TextFormattingMode GetTextFormattingMode([CanBeNull] IContextBoundSettingsStore settings)
			=> settings?.GetValue((DisplaySettings s) => s.TextFormattingMode) ?? TextFormattingMode.Ideal;

		private static void RestoreOriginalStyles([NotNull] ItemsControl listBox) {
			if (!(listBox.GetValue(_originalStylesProperty) is OriginalStyles originalStyles))
				return;

			listBox.ClearValue(_originalStylesProperty);

			listBox.Style = originalStyles.Style;
			listBox.ItemContainerStyle = originalStyles.ItemContainerStyle;
			listBox.ItemTemplate = originalStyles.ItemTemplate;
		}

	}

}