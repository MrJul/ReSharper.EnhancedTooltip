using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.UI.RichText;
using Microsoft.VisualStudio.Text.Formatting;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public class MultipleTooltipContentPresenter {

		private readonly List<IdentifierTooltipContent> _identifierContents = new();
		private readonly List<ArgumentRoleTooltipContent> _argumentRoleContents = new();
		private readonly List<IssueTooltipContent> _issueContents = new();
		private readonly List<MiscTooltipContent> _miscContents = new();
		private readonly List<VsIdentifierContent> _vsIdentifierContents = new();
		private readonly List<VsSquiggleContent> _vsSquiggleContents = new();
		private readonly List<object> _vsUnknownContents = new();
		private readonly TextFormattingRunProperties _formatting;
		private readonly IDocument? _document;

		[ContractAnnotation("null => false")]
		public bool TryAddReSharperContent(IReSharperTooltipContent? content) {
			if (content is null || content.Text.IsNullOrEmpty())
				return false;

			switch (content) {
				case IdentifierTooltipContent identifierContent:
					AddIdentifierTooltipContent(identifierContent);
					return true;
				case ArgumentRoleTooltipContent argumentRoleContent:
					AddArgumentRoleTooltipContent(argumentRoleContent);
					return true;
				case IssueTooltipContent issueContent:
					AddIssueTooltipContent(issueContent);
					return true;
				case MiscTooltipContent miscContent:
					AddMiscTooltipContent(miscContent);
					return true;
				default:
					return false;
			}
		}

		public void AddIdentifierTooltipContent(IdentifierTooltipContent content)
			=> _identifierContents.Add(content);

		public void AddArgumentRoleTooltipContent(ArgumentRoleTooltipContent content)
			=> _argumentRoleContents.Add(content);

		public void AddIssueTooltipContent(IssueTooltipContent content)
			=> _issueContents.Add(content);

		public void AddMiscTooltipContent(MiscTooltipContent content)
			=> _miscContents.Add(content);

		public void AddVsSquiggleContent(VsSquiggleContent content)
			=> _vsSquiggleContents.Add(content);

		public void AddVsIdentifierContent(VsIdentifierContent content)
			=> _vsIdentifierContents.Add(content);

		public void AddVsUnknownContent(object content)
			=> _vsUnknownContents.Add(content);

		public IEnumerable<object> PresentContents() {
			foreach (IdentifierTooltipContent identifierContent in _identifierContents)
				yield return PresentTooltipContents("Id", new[] { identifierContent });

			foreach (VsIdentifierContent identifierContent in _vsIdentifierContents)
				yield return PresentTooltipContents("Id", new[] { identifierContent });

			foreach (ArgumentRoleTooltipContent argumentRoleContent in _argumentRoleContents)
				yield return PresentTooltipContents("Role", new[] { argumentRoleContent });

			if (_issueContents.Count > 0)
				yield return PresentTooltipContents(_issueContents.Count == 1 ? "Issue" : "Issues", _issueContents);

			foreach (MiscTooltipContent miscContent in _miscContents)
				yield return PresentTooltipContents("Misc", new[] { miscContent });

			foreach (object vsUnknownContent in _vsUnknownContents)
				yield return vsUnknownContent;

			if (_vsSquiggleContents.Count > 0)
				yield return PresentTooltipContents("VS", _vsSquiggleContents);
		}

		[Pure]
		private HeaderedContentControl PresentTooltipContents(object? header, IEnumerable<object> tooltipContents) {
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
			Styling.SetDocument(control, new WeakReference<IDocument?>(_document));
			return control;
		}

		private void ApplyFormatting(Control control) {
			if (!_formatting.TypefaceEmpty) {
				Typeface typeface = _formatting.Typeface;
				control.FontFamily = typeface.FontFamily;
				control.FontWeight = typeface.Weight;
				control.FontStretch = typeface.Stretch;
				control.FontStyle = typeface.Style;
			}
			
			if (!_formatting.FontRenderingEmSizeEmpty)
				control.FontSize = _formatting.FontRenderingEmSize;
		}

		public MultipleTooltipContentPresenter(TextFormattingRunProperties formatting, IDocument? document) {
			_formatting = formatting;
			_document = document;
		}

	}

}