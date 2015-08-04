using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.VisualStudio;
using JetBrains.Annotations;
using JetBrains.Platform.VisualStudio.SinceVs11.Shell.Theming;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.UI.Avalon;
using JetBrains.UI.Extensions;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public static class Styling {

		/// <summary>
		/// Identifies the attached dependency property <c>IsTransparent</c>.
		/// </summary>
		public static readonly DependencyProperty ShouldStyleParentListBoxProperty = DependencyProperty.RegisterAttached(
			"ShouldStyleParentListBox",
			typeof(bool),
			typeof(Styling),
			new FrameworkPropertyMetadata(BooleanBoxes.False, OnShouldStyleParentListBoxChanged));

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

		private static void OnShouldStyleParentListBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(bool) e.NewValue)
				return;

			var element = d as FrameworkElement;
			
			element?.WhenLoaded(lifetime => {

				// We're styling the parent VS ListBox included inside the tooltip.
				var listBox = element.FindVisualAncestor<ListBox>();
				if (listBox == null || listBox.GetValue(_originalStylesProperty) != null)
					return;

				SetListBoxStyle(listBox);

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

		private static void SetListBoxStyle([NotNull] ListBox listBox) {
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

			var itemContainerStyle = new Style(typeof(ListBoxItem), UIResources.Instance.QuickInfoItemStyle);
			if (Shell.HasInstance) {
				var tooltipFormattingProvider = Shell.Instance.TryGetComponent<TooltipFormattingProvider>();

				if (tooltipFormattingProvider != null) {

					var backgroundBrush = tooltipFormattingProvider.TryGetBackgroundBrush()
						?? listBox.FindResource(BundledThemeColors.Environment.ToolWindowTabSelectedTabBrushKey) as Brush;
					if (backgroundBrush != null)
						itemContainerStyle.Setters.Add(new Setter(Control.BackgroundProperty, backgroundBrush));

					var foregroundBrush = tooltipFormattingProvider.TryGetForegroundBrush();
					if (foregroundBrush != null)
						itemContainerStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, foregroundBrush));

				}

			}
			itemContainerStyle.Seal();
			
			listBox.ItemContainerStyle = itemContainerStyle;

			//Shell.Instance.GetComponent<TooltipFormattingProvider>().GetTextFormatting().BackgroundBrush;
		}

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