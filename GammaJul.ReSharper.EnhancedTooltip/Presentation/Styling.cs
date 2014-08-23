using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
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

		public static bool GetShouldStyleParentListBox([NotNull] DependencyObject owner) {
			if (owner == null)
				throw new ArgumentNullException("owner");
			return (bool) owner.GetValue(ShouldStyleParentListBoxProperty);
		}

		public static void SetShouldStyleParentListBox([NotNull] DependencyObject owner, bool value) {
			if (owner == null)
				throw new ArgumentNullException("owner");
			owner.SetValue(ShouldStyleParentListBoxProperty, value);
		}

		private static void OnShouldStyleParentListBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(bool) e.NewValue)
				return;

			var element = d as FrameworkElement;
			if (element == null)
				return;
			
			element.WhenLoaded(lifetime => {
				// We're styling the parent VS ListBox included inside the tooltip.
				var listBox = element.FindVisualAncestor<ListBox>();
				if (listBox != null)
					SetListBoxStyle(listBox);
			});
		}

		private static void SetListBoxStyle([NotNull] ListBox listBox) {
			listBox.Style = UIResources.Instance.QuickInfoListBoxStyle;
			listBox.ItemContainerStyle = UIResources.Instance.QuickInfoItemStyle;
			listBox.ItemTemplate = UIResources.Instance.QuickInfoItemDataTemplate;
		}

	}

}