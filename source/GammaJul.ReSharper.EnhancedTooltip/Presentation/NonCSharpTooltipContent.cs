using System;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VsIntegration.TextControl;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

  public class NonCSharpTooltipContent {
    public Object Content { get; set; }
    public NonCSharpTooltipContent(Object content) {
      this.Content = content;
    }
  }

}