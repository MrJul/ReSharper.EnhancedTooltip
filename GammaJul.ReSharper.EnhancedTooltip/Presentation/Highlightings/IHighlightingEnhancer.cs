using System;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.UI.RichText;
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings {

	public interface IHighlightingEnhancer {

		[NotNull]
		Type HighlightingType { get; }

		[CanBeNull]
		RichText TryEnhance([NotNull] IHighlighting highlighting, [NotNull] IContextBoundSettingsStore settings);

	}

}