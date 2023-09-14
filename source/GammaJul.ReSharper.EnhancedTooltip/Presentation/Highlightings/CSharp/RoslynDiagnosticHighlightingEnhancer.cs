using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

  using System;

  using JetBrains.PsiFeatures.VisualStudio.Backend.Daemon;
  using JetBrains.Reflection;

  [SolutionComponent]
  internal sealed class RoslynDiagnosticHighlightingEnhancer : CSharpHighlightingEnhancer<RoslynDiagnosticsDaemonProcess.RoslynDiagnosticHighlighting> {

    protected override void AppendTooltip(RoslynDiagnosticsDaemonProcess.RoslynDiagnosticHighlighting highlighting, CSharpColorizer colorizer) {
      var originalTooltip = highlighting.GetDynamicFieldOrProperty("ToolTip").ToString();
      var destinationTooltip = originalTooltip.Replace(highlighting.CompilerId + ": ", String.Empty);
      colorizer.AppendPlainText(destinationTooltip);
      colorizer.AppendPlainText(" [Roslyn Rule: ");
      colorizer.AppendPlainText(highlighting.CompilerId);
      colorizer.AppendPlainText("]");
    }

    public RoslynDiagnosticHighlightingEnhancer(
      TextStyleHighlighterManager textStyleHighlighterManager,
      CodeAnnotationsConfiguration codeAnnotationsConfiguration,
      HighlighterIdProviderFactory highlighterIdProviderFactory)
      : base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
    }

  }

}