using System;
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

		public static readonly DependencyProperty IdentifierTextProperty = DependencyProperty.Register(
			nameof(IdentifierText),
			typeof(RichText),
			typeof(IdentifierPresenter),
			new FrameworkPropertyMetadata(null, (d, e) => (d as IdentifierPresenter)?.OnRichTextChanged(e.NewValue as RichText)));

		public static readonly DependencyProperty OverloadCountProperty = DependencyProperty.Register(
			nameof(OverloadCount),
			typeof(int?),
			typeof(IdentifierPresenter),
			new FrameworkPropertyMetadata(null, (d, e) => (d as IdentifierPresenter)?.OnOverloadCountChanged(e.NewValue as int?)));

		private Span? _richTextSpan;
		private Run? _overloadCountRun;

		public RichText? IdentifierText {
			get => (RichText) GetValue(IdentifierTextProperty);
			set => SetValue(IdentifierTextProperty, value);
		}

		public int? OverloadCount {
			get => (int?) GetValue(OverloadCountProperty);
			set => SetValue(OverloadCountProperty, value);
		}

		private void OnRichTextChanged(RichText? newValue) {
			_richTextSpan = newValue?.ToSpan();
			SetInlines();
		}

		private void OnOverloadCountChanged(int? newValue) {
			string? text = GetOverloadCountText(newValue);
			_overloadCountRun = text is null ? null : CreateOverloadCountRun(text);
			SetInlines();
		}

		private Run CreateOverloadCountRun(string text)
			=> new Run(text) {
				Foreground = Brushes.Gray,
				FontSize = FontSize * 0.9,
				FontStyle = FontStyles.Italic
			};

		[Pure]
		private static string? GetOverloadCountText(int? overloadCount)
			=> overloadCount switch {
				null => null,
				0 => null,
				1 => " + 1 overload",
				{ } value => $" + {value} overloads"
			};

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			SetInlines();
		}

		private void SetInlines() {
			if (!IsInitialized)
				return;

			var inlines = Inlines;
			inlines.Clear();
			if (_richTextSpan is not null)
				inlines.Add(_richTextSpan);
			if (_overloadCountRun is not null)
				inlines.Add(_overloadCountRun);
		}

	}

}