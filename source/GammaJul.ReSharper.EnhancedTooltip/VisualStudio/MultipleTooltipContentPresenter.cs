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
    private readonly List<NonCSharpTooltipContent> _vsNonCsharpContents = new();
    private readonly TextFormattingRunProperties _formatting;
    private readonly IDocument? _document;

    [ContractAnnotation("null => false")]
    public bool TryAddReSharperContent(IReSharperTooltipContent? content) {
      if (content is null || content.Text.IsNullOrEmpty())
        return false;

      switch (content) {
        case IdentifierTooltipContent identifierContent:
          this.AddIdentifierTooltipContent(identifierContent);
          return true;
        case ArgumentRoleTooltipContent argumentRoleContent:
          this.AddArgumentRoleTooltipContent(argumentRoleContent);
          return true;
        case IssueTooltipContent issueContent:
          this.AddIssueTooltipContent(issueContent);
          return true;
        case MiscTooltipContent miscContent:
          this.AddMiscTooltipContent(miscContent);
          return true;
        default:
          return false;
      }
    }

    public void AddIdentifierTooltipContent(IdentifierTooltipContent content)
      => this._identifierContents.Add(content);

    public void AddArgumentRoleTooltipContent(ArgumentRoleTooltipContent content)
      => this._argumentRoleContents.Add(content);

    public void AddIssueTooltipContent(IssueTooltipContent content)
      => this._issueContents.Add(content);

    public void AddMiscTooltipContent(MiscTooltipContent content)
      => this._miscContents.Add(content);

    public void AddVsSquiggleContent(VsSquiggleContent content)
      => this._vsSquiggleContents.Add(content);

    public void AddVsIdentifierContent(VsIdentifierContent content)
      => this._vsIdentifierContents.Add(content);

    public void AddVsUnknownContent(object content)
      => this._vsUnknownContents.Add(content);
    public void AddNonCSharpContent(NonCSharpTooltipContent content)
      => this._vsNonCsharpContents.Add(content);

    public IEnumerable<object> PresentContents() {
      if (this._vsNonCsharpContents.Count > 0) {
        yield return this.PresentTooltipContents("VSNonC#", this._vsNonCsharpContents, this._vsIdentifierContents.Count == 0 && this._argumentRoleContents.Count == 0 && this._issueContents.Count == 0 && this._miscContents.Count == 0 && this._vsUnknownContents.Count == 0 && this._vsSquiggleContents.Count == 0);
      }

      foreach (IdentifierTooltipContent identifierContent in this._identifierContents)
        yield return this.PresentTooltipContents("Id", new[] { identifierContent }, this._vsNonCsharpContents.Count == 0 && this._vsIdentifierContents.Count == 0 && this._argumentRoleContents.Count == 0 && this._issueContents.Count == 0 && this._miscContents.Count == 0 && this._vsUnknownContents.Count == 0 && this._vsSquiggleContents.Count == 0);

      foreach (VsIdentifierContent identifierContent in this._vsIdentifierContents)
        yield return this.PresentTooltipContents("Id", new[] { identifierContent }, this._argumentRoleContents.Count == 0 && this._issueContents.Count == 0 && this._miscContents.Count == 0 && this._vsUnknownContents.Count == 0 && this._vsSquiggleContents.Count == 0);

      foreach (ArgumentRoleTooltipContent argumentRoleContent in this._argumentRoleContents)
        yield return this.PresentTooltipContents("Role", new[] { argumentRoleContent }, this._issueContents.Count == 0 && this._miscContents.Count == 0 && this._vsUnknownContents.Count == 0 && this._vsSquiggleContents.Count == 0);

      if (this._issueContents.Count > 0)
        yield return this.PresentTooltipContents(this._issueContents.Count == 1 ? "Issue" : "Issues", this._issueContents, this._miscContents.Count == 0 && this._vsUnknownContents.Count == 0 && this._vsSquiggleContents.Count == 0);

      foreach (MiscTooltipContent miscContent in this._miscContents)
        yield return this.PresentTooltipContents("Misc", new[] { miscContent }, this._vsUnknownContents.Count == 0 && this._vsSquiggleContents.Count == 0);

      if (this._vsSquiggleContents.Count > 0)
        yield return this.PresentTooltipContents("VS", this._vsSquiggleContents, this._vsUnknownContents.Count == 0);

      foreach (object vsUnknownContent in this._vsUnknownContents)
        yield return vsUnknownContent;
    }

    [Pure]
    private HeaderedContentControl PresentTooltipContents(object? header, IEnumerable<object> tooltipContents, Boolean isLast = true) {
      var control = new HeaderedContentControl {
        Style = UIResources.Instance.HeaderedContentControlStyle,
        Focusable = false,
        Tag = isLast ? "Last" : "",
        Header = header,
        Content = new ItemsControl {
          Focusable = false,
          ItemsSource = tooltipContents
        }
      };
      this.ApplyFormatting(control);
      Styling.SetDocument(control, new WeakReference<IDocument?>(this._document));
      return control;
    }

    private void ApplyFormatting(Control control) {
      if (!this._formatting.TypefaceEmpty) {
        Typeface typeface = this._formatting.Typeface;
        control.FontFamily = typeface.FontFamily;
        control.FontWeight = typeface.Weight;
        control.FontStretch = typeface.Stretch;
        control.FontStyle = typeface.Style;
      }

      if (!this._formatting.FontRenderingEmSizeEmpty)
        control.FontSize = this._formatting.FontRenderingEmSize;
    }

    public MultipleTooltipContentPresenter(TextFormattingRunProperties formatting, IDocument? document) {
      this._formatting = formatting;
      this._document = document;
    }

  }

}