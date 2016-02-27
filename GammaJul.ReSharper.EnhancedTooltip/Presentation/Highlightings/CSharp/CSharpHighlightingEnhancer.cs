﻿using System;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	internal abstract class CSharpHighlightingEnhancer<T> : IHighlightingEnhancer
	where T : class, IHighlighting {

		[NotNull] private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		[NotNull] private readonly CodeAnnotationsCache _codeAnnotationsCache;
		[NotNull] private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;

		public Type HighlightingType
			=> typeof(T);

		public RichText TryEnhance(IHighlighting highlighting, IContextBoundSettingsStore settings) {
			var typedHighlighting = highlighting as T;
			if (typedHighlighting == null)
				return null;
			
			var richText = new RichText();
			var highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);
			var colorizer = new CSharpColorizer(richText, _textStyleHighlighterManager, _codeAnnotationsCache, highlighterIdProvider);
			AppendTooltip(typedHighlighting, colorizer);
			return richText;
		}

		protected abstract void AppendTooltip([NotNull] T highlighting, [NotNull] CSharpColorizer colorizer);

		protected CSharpHighlightingEnhancer(
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache,
			HighlighterIdProviderFactory highlighterIdProviderFactory) {
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsCache = codeAnnotationsCache;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}