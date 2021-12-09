using System;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings {

	public interface IHighlightingEnhancer {

		Type HighlightingType { get; }

		RichText? TryEnhance(IHighlighting highlighting, IContextBoundSettingsStore settings);

	}

}