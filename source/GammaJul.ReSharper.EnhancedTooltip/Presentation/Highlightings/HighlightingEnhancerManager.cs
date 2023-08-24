using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings {

  [SolutionComponent]
  public sealed class HighlightingEnhancerManager {
    private readonly Dictionary<Type, IHighlightingEnhancer> _HighlightingEnhancers;
    public RichText? TryEnhance(IHighlighting? highlighting, IContextBoundSettingsStore settings) {
      if (highlighting is null || !this._HighlightingEnhancers.TryGetValue(highlighting.GetType(), out IHighlightingEnhancer highlightingEnhancer)) {
        return null;
      }

      return highlightingEnhancer.TryEnhance(highlighting, settings);
    }

    public HighlightingEnhancerManager(IEnumerable<IHighlightingEnhancer> highlightingEnhancers) {
      this._HighlightingEnhancers = highlightingEnhancers.ToDictionary(he => he.HighlightingType);
    }

  }

}