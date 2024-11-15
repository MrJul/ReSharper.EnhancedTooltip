using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

  using System;
  using System.Linq;
  using GammaJul.ReSharper.EnhancedTooltip.Psi;
  using JetBrains.ReSharper.Daemon.UsageChecking;


  [SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
  internal sealed class UnusedMemberLocalWarningEnhancer : CSharpHighlightingEnhancer<UnusedMemberLocalWarning> {

    protected override void AppendTooltip(UnusedMemberLocalWarning highlighting, CSharpColorizer colorizer) {
      var kind = highlighting.Declaration.DeclaredElement.GetElementKindString(false, false, false, false, false);

      colorizer.AppendPlainText(Char.ToUpper(kind.First()) + kind.Substring(1).ToLower());
      colorizer.AppendPlainText(" '");
      colorizer.AppendDeclaredElement(
        highlighting.Declaration.DeclaredElement!,
        highlighting.Declaration.DeclaredElement.GetIdSubstitutionSafe(),
        PresenterOptions.NameOnly,
        null);
      colorizer.AppendPlainText("' is never used");
    }

    public UnusedMemberLocalWarningEnhancer(
      TextStyleHighlighterManager textStyleHighlighterManager,
      CodeAnnotationsConfiguration codeAnnotationsConfiguration,
      HighlighterIdProviderFactory highlighterIdProviderFactory)
      : base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
    }

  }

}