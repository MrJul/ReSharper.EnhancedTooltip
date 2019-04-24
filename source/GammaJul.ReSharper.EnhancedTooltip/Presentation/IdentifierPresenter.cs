using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using JetBrains.Annotations;
using JetBrains.UI.Controls;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// Similar to <see cref="RichTextPresenter"/>, but specialized for identifiers, with optional overload count display.
	/// </summary>
	public class IdentifierPresenter : TextBlock {

		[NotNull]
		public static readonly DependencyProperty IdentifierTextProperty = DependencyProperty.Register(
			nameof(IdentifierText),
			typeof(RichText),
			typeof(IdentifierPresenter),
			new FrameworkPropertyMetadata(null, (d, e) => (d as IdentifierPresenter)?.OnRichTextChanged(e.NewValue as RichText)));

		[NotNull]
		public static readonly DependencyProperty OverloadCountProperty = DependencyProperty.Register(
			nameof(OverloadCount),
			typeof(int?),
			typeof(IdentifierPresenter),
			new FrameworkPropertyMetadata(null, (d, e) => (d as IdentifierPresenter)?.OnOverloadCountChanged(e.NewValue as int?)));

		[CanBeNull] private Span _richTextSpan;
		[CanBeNull] private Run _overloadCountRun;

		[CanBeNull]
		public RichText IdentifierText {
			get => (RichText) GetValue(IdentifierTextProperty);
			set => SetValue(IdentifierTextProperty, value);
		}

		public int? OverloadCount {
			get => (int?) GetValue(OverloadCountProperty);
			set => SetValue(OverloadCountProperty, value);
		}

		private void OnRichTextChanged([CanBeNull] RichText newValue) {
			_richTextSpan = newValue?.ToSpan();
			SetInlines();
		}

		private void OnOverloadCountChanged(int? newValue) {
			string text = GetOverloadCountText(newValue);
			_overloadCountRun = text == null ? null : CreateOverloadCountRun(text);
			SetInlines();
		}

		[CanBeNull]
		private Run CreateOverloadCountRun([NotNull] string text)
			=> new Run(text) {
				Foreground = Brushes.Gray,
				FontSize = FontSize * 0.9,
				FontStyle = FontStyles.Italic
			};

		[CanBeNull]
		[Pure]
		private static string GetOverloadCountText(int? overloadCount) {
			switch (overloadCount) {
				case null: return null;
				case 0: return null;
				case 1: return " + 1 overload";
				default: return String.Format(CultureInfo.CurrentCulture, " + {0} overloads", overloadCount.Value);
			}
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			SetInlines();
		}

		private void SetInlines() {
			if (!IsInitialized)
				return;

			var inlines = Inlines;
			inlines.Clear();
			if (_richTextSpan != null)
				inlines.Add(_richTextSpan);
			if (_overloadCountRun != null)
				inlines.Add(_overloadCountRun);
		}

	}

}