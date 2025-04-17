using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.ExternalHighlightings;
using GammaJul.ReSharper.EnhancedTooltip.Utils;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings
{

  [SolutionComponent(Instantiation.ContainerAsyncAnyThreadUnsafe)]
  public sealed class HighlightingEnhancerManager {
    private readonly Dictionary<Type, IHighlightingEnhancer> _HighlightingEnhancers;
    public RichText? TryEnhance(IHighlighting? highlighting, IContextBoundSettingsStore settings) {
      var highlightingType = highlighting?.GetType().GetExternalHighlightingType();
      if (highlighting is null || !this._HighlightingEnhancers.TryGetValue(highlightingType ?? highlighting.GetType(), out IHighlightingEnhancer highlightingEnhancer)) {
        return null;
      }

      return highlightingEnhancer.TryEnhance(highlighting, settings);
    }

    public HighlightingEnhancerManager(IEnumerable<IHighlightingEnhancer> highlightingEnhancers) {
      this._HighlightingEnhancers = highlightingEnhancers.ToDictionary(he => he.HighlightingType);
    }

  }

}