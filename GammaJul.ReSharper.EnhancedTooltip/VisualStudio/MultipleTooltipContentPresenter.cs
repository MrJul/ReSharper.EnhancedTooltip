using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.UI.RichText;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class MultipleTooltipContentPresenter {

		[NotNull] private readonly List<IdentifierTooltipContent> _identifierContents = new List<IdentifierTooltipContent>();
		[NotNull] private readonly List<IssueTooltipContent> _issueContents = new List<IssueTooltipContent>();
		[NotNull] private readonly List<MiscTooltipContent> _miscContents = new List<MiscTooltipContent>();
		[NotNull] private readonly List<VsIdentifierContent> _vsIdentifierContents = new List<VsIdentifierContent>();
		[NotNull] private readonly List<VsSquiggleContent> _vsSquiggleContents = new List<VsSquiggleContent>();
		[NotNull] private readonly List<object> _vsUnknownContents = new List<object>();
		[NotNull] private readonly TextFormattingRunProperties _formatting;

		[ContractAnnotation("null => false")]
		public bool TryAddReSharperContent([CanBeNull] IReSharperTooltipContent content) {
			if (content == null || content.Text.IsNullOrEmpty())
				return false;

			var identifierContent = content as IdentifierTooltipContent;
			if (identifierContent != null) {
				_identifierContents.Add(identifierContent);
				return true;
			}

			var issueContent = content as IssueTooltipContent;
			if (issueContent != null) {
				_issueContents.Add(issueContent);
				return true;
			}

			var rsContent = content as MiscTooltipContent;
			if (rsContent != null) {
				_miscContents.Add(rsContent);
				return true;
			}

			return false;
		}

		public void AddVsSquiggleContent([NotNull] VsSquiggleContent content)
			=> _vsSquiggleContents.Add(content);

		public void AddVsIdentifierContent([NotNull] VsIdentifierContent content)
			=> _vsIdentifierContents.Add(content);

		public void AddVsUnknownContent([NotNull] object content)
			=> _vsUnknownContents.Add(content);

		[NotNull]
		public IEnumerable<object> PresentContents() {
			foreach (IdentifierTooltipContent identifierContent in _identifierContents)
				yield return PresentTooltipContents("Id", new[] { identifierContent });

			foreach (VsIdentifierContent identifierContent in _vsIdentifierContents)
				yield return PresentTooltipContents("Id", new[] { identifierContent });

			if (_issueContents.Count > 0)
				yield return PresentTooltipContents(_issueContents.Count == 1 ? "Issue" : "Issues", _issueContents);

			foreach (MiscTooltipContent miscContent in _miscContents)
				yield return PresentTooltipContents("Misc", new[] { miscContent });

			foreach (object vsUnknownContent in _vsUnknownContents)
				yield return vsUnknownContent;

			if (_vsSquiggleContents.Count > 0)
				yield return PresentTooltipContents("VS", _vsSquiggleContents);
		}

		[NotNull]
		[Pure]
		private HeaderedContentControl PresentTooltipContents([CanBeNull] object header, [NotNull] IEnumerable<object> tooltipContents) {
			var control = new HeaderedContentControl {
				Style = UIResources.Instance.HeaderedContentControlStyle,
				Focusable = false,
				Header = header,
				Content = new ItemsControl {
					Focusable = false,
					ItemsSource = tooltipContents
				}
			};
			ApplyFormatting(control);
			return control;
		}

		private void ApplyFormatting([NotNull] Control control) {
			if (!_formatting.TypefaceEmpty) {
				Typeface typeface = _formatting.Typeface;
				control.FontFamily = typeface.FontFamily;
				control.FontWeight = typeface.Weight;
				control.FontStretch = typeface.Stretch;
				control.FontStyle = typeface.Style;
			}
			if (!_formatting.FontRenderingEmSizeEmpty)
				control.FontSize = _formatting.FontRenderingEmSize;
//			if (!_formatting.ForegroundBrushEmpty)
//				control.Foreground = _formatting.ForegroundBrush;
		}

		public MultipleTooltipContentPresenter([NotNull] TextFormattingRunProperties formatting) {
			_formatting = formatting;
		}

	}

}