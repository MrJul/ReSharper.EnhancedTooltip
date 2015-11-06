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
using JetBrains.DocumentModel;
using JetBrains.Platform.VisualStudio.SinceVs11.Shell.Theming;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.UI.Avalon;
using JetBrains.UI.Extensions;
using JetBrains.Util;
using JetBrains.Util.Interop;
using Screen = System.Windows.Forms.Screen;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public static class Styling {

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
			new FrameworkPropertyMetadata(null));

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
			
			element?.WhenLoaded(lifetime => {

				// We're styling the parent VS ListBox included inside the tooltip.
				var listBox = element.FindVisualAncestor<ListBox>();
				if (listBox == null || listBox.GetValue(_originalStylesProperty) != null)
					return;

				IDocument document = null;
				GetDocument(element)?.TryGetTarget(out document);

				SetListBoxStyle(listBox, document);

				RoutedEventHandler onListBoxUnloaded = null;
				onListBoxUnloaded = (sender, args) => {
					listBox.Unloaded -= onListBoxUnloaded;
					RestoreOriginalStyles(listBox);
				};
				listBox.Unloaded += onListBoxUnloaded;

			});
		}


		private sealed class OriginalStyles {
			[CanBeNull] public Style Style { get; set; }
			[CanBeNull] public Style ItemContainerStyle { get; set; }
			[CanBeNull] public DataTemplate ItemTemplate { get; set; }
		}

		private static void SetListBoxStyle([NotNull] ListBox listBox, [CanBeNull] IDocument document) {
			var originalStyles = listBox.GetValue(_originalStylesProperty) as OriginalStyles;
			if (originalStyles == null) {
				listBox.SetValue(_originalStylesProperty, new OriginalStyles {
					Style = listBox.Style,
					ItemContainerStyle = listBox.ItemContainerStyle,
					ItemTemplate = listBox.ItemTemplate
				});
			}

			listBox.Style = UIResources.Instance.QuickInfoListBoxStyle;
			listBox.ItemTemplate = UIResources.Instance.QuickInfoItemDataTemplate;
			listBox.ItemContainerStyle = CreateItemContainerStyle(listBox);

			IContextBoundSettingsStore settings = document.TryGetSettings();
			listBox.MaxWidth = ComputeListBoxMaxWidth(listBox,  settings);
			TextOptions.SetTextFormattingMode(listBox, GetTextFormattingMode(settings));
			TextOptions.SetTextRenderingMode(listBox, TextRenderingMode.Auto);
		}

		[NotNull]
		private static Style CreateItemContainerStyle([NotNull] FrameworkElement resourceSource) {
			var itemContainerStyle = new Style(typeof(ListBoxItem), UIResources.Instance.QuickInfoItemStyle);

			if (Shell.HasInstance) {
				var tooltipFormattingProvider = Shell.Instance.TryGetComponent<TooltipFormattingProvider>();
				if (tooltipFormattingProvider != null) {

					var backgroundBrush = tooltipFormattingProvider.TryGetBackgroundBrush()
						?? resourceSource.FindResource(BundledThemeColors.Environment.ToolWindowTabSelectedTabBrushKey) as Brush;
					if (backgroundBrush != null)
						itemContainerStyle.Setters.Add(new Setter(ItemTemplateBackgroundProperty, backgroundBrush));

					var foregroundBrush = tooltipFormattingProvider.TryGetForegroundBrush();
					if (foregroundBrush != null)
						itemContainerStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, foregroundBrush));

				}
			}

			itemContainerStyle.Seal();
			return itemContainerStyle;
		}

		private static double ComputeListBoxMaxWidth([NotNull] ListBox listBox, [CanBeNull] IContextBoundSettingsStore settings) {
			if (settings == null || !settings.GetValue((DisplaySettings s) => s.LimitTooltipWidth))
				return Double.PositiveInfinity;

			var hwndSource = PresentationSource.FromVisual(listBox) as HwndSource;
			if (hwndSource == null)
				return Double.PositiveInfinity;

			int limitPercent = settings.GetValue((DisplaySettings s) => s.ScreenWidthLimitPercent).Clamp(10, 100);
			Screen screen = Screen.FromHandle(hwndSource.Handle);
			return screen.Bounds.Width * (limitPercent / 100.0) / DpiUtil.DpiHorizontalFactor;
		}

		private static TextFormattingMode GetTextFormattingMode([CanBeNull] IContextBoundSettingsStore settings)
			=> settings?.GetValue((DisplaySettings s) => s.TextFormattingMode) ?? TextFormattingMode.Ideal;

		private static void RestoreOriginalStyles([NotNull] ListBox listBox) {
			var originalStyles = listBox.GetValue(_originalStylesProperty) as OriginalStyles;
			if (originalStyles == null)
				return;

			listBox.ClearValue(_originalStylesProperty);

			listBox.Style = originalStyles.Style;
			listBox.ItemContainerStyle = originalStyles.ItemContainerStyle;
			listBox.ItemTemplate = originalStyles.ItemTemplate;
		}

	}

}