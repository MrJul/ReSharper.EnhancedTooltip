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

		[NotNull] private readonly List<IdentifierTooltipContent> _identifierContents = new List<IdentifierTooltipContent>();
		[NotNull] private readonly List<ArgumentRoleTooltipContent> _argumentRoleContents = new List<ArgumentRoleTooltipContent>();
		[NotNull] private readonly List<IssueTooltipContent> _issueContents = new List<IssueTooltipContent>();
		[NotNull] private readonly List<MiscTooltipContent> _miscContents = new List<MiscTooltipContent>();
		[NotNull] private readonly List<VsIdentifierContent> _vsIdentifierContents = new List<VsIdentifierContent>();
		[NotNull] private readonly List<VsSquiggleContent> _vsSquiggleContents = new List<VsSquiggleContent>();
		[NotNull] private readonly List<object> _vsUnknownContents = new List<object>();
		[NotNull] private readonly TextFormattingRunProperties _formatting;
		[CanBeNull] private readonly IDocument _document;

		[ContractAnnotation("null => false")]
		public bool TryAddReSharperContent([CanBeNull] IReSharperTooltipContent content) {
			if (content == null || content.Text.IsNullOrEmpty())
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

		public void AddIdentifierTooltipContent([NotNull] IdentifierTooltipContent content)
			=> _identifierContents.Add(content);

		public void AddArgumentRoleTooltipContent([NotNull] ArgumentRoleTooltipContent content)
			=> _argumentRoleContents.Add(content);

		public void AddIssueTooltipContent([NotNull] IssueTooltipContent content)
			=> _issueContents.Add(content);

		public void AddMiscTooltipContent([NotNull] MiscTooltipContent content)
			=> _miscContents.Add(content);

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
			Styling.SetDocument(control, new WeakReference<IDocument>(_document));
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
		}

		public MultipleTooltipContentPresenter([NotNull] TextFormattingRunProperties formatting, [CanBeNull] IDocument document) {
			_formatting = formatting;
			_document = document;
		}

	}

}