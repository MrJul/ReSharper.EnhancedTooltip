using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

  using GammaJul.ReSharper.EnhancedTooltip.ExceptionalHighlightings;
  using GammaJul.ReSharper.EnhancedTooltip.Utils;
  using JetBrains.ReSharper.Feature.Services.Daemon;

  [SolutionComponent]
  internal sealed class ExceptionNotDocumentedHighlightingEnhancer : CSharpHighlightingEnhancer<IExceptionNotDocumentedHighlighting> {

    protected override void AppendTooltip(IExceptionNotDocumentedHighlighting highlighting, CSharpColorizer colorizer) {  }

    protected override void AppendTooltip(IHighlighting highlighting, CSharpColorizer colorizer) {
      var typeName = highlighting.GetDynamicFieldOrPropertySafe("ExceptionTypeName");
      if (typeName != null) {
        colorizer.AppendPlainText("(optional) Exception '");
        colorizer.AppendClassName(typeName.ToString());
        colorizer.AppendPlainText("' is not documented [Exceptional]");
      } else {
        colorizer.AppendPlainText(highlighting.ToolTip);
      }
    }

    public ExceptionNotDocumentedHighlightingEnhancer(
      TextStyleHighlighterManager textStyleHighlighterManager,
      CodeAnnotationsConfiguration codeAnnotationsConfiguration,
      HighlighterIdProviderFactory highlighterIdProviderFactory)
      : base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
    }

  }

}