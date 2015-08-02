using System;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings {

	public interface IHighlightingEnhancer {

		[NotNull]
		Type HighlightingType { get; }

		[CanBeNull]
		RichText TryEnhance([NotNull] IHighlighting highlighting, [NotNull] IContextBoundSettingsStore settings);

	}

}