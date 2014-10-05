using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class MultipleTooltipContentPresenter {

		private readonly List<IdentifierTooltipContent> _identifierContents = new List<IdentifierTooltipContent>();
		private readonly List<IssueTooltipContent> _issueContents = new List<IssueTooltipContent>();
		private readonly List<ReSharperTooltipContent> _otherReSharperContents = new List<ReSharperTooltipContent>();
		private readonly List<VisualStudioTooltipContent> _visualStudioContents = new List<VisualStudioTooltipContent>();
		private readonly TextFormattingRunProperties _formatting;

		public bool TryAddContent([CanBeNull] ITooltipContent content) {
			if (content == null || content.IsEmpty)
				return true;

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

			var rsContent = content as ReSharperTooltipContent;
			if (rsContent != null) {
				_otherReSharperContents.Add(rsContent);
				return true;
			}

			var vsContent = content as VisualStudioTooltipContent;
			if (vsContent != null) {
				_visualStudioContents.Add(vsContent);
				return true;
			}

			return false;
		}
		
		[NotNull]
		public IEnumerable<UIElement> PresentContents() {
			foreach (IdentifierTooltipContent identifierContent in _identifierContents)
				yield return PresentTooltipContents("Id", new[] { identifierContent });

			if (_issueContents.Count > 0)
				yield return PresentTooltipContents(_issueContents.Count == 1 ? "Issue" : "Issues", _issueContents);

			foreach (ReSharperTooltipContent rsContent in _otherReSharperContents)
				yield return PresentTooltipContents("Misc", new[] { rsContent });

			foreach (VisualStudioTooltipContent vsContent in _visualStudioContents)
				yield return PresentTooltipContents("Misc", new[] { vsContent });
		}

		[NotNull]
		[Pure]
		private HeaderedContentControl PresentTooltipContents([CanBeNull] object header, [NotNull] IEnumerable<ITooltipContent> tooltipContents) {
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
			if (!_formatting.ForegroundBrushEmpty)
				control.Foreground = _formatting.ForegroundBrush;
		}

		public MultipleTooltipContentPresenter([NotNull] TextFormattingRunProperties formatting) {
			_formatting = formatting;
		}

	}

}