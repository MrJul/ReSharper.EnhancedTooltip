using System;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	internal abstract class CSharpHighlightingEnhancer<T> : IHighlightingEnhancer
	where T : class, IHighlighting {

		private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		private readonly CodeAnnotationsConfiguration _codeAnnotationsConfiguration;
		private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;

		public Type HighlightingType
			=> typeof(T);
		
		public RichText? TryEnhance(IHighlighting highlighting, IContextBoundSettingsStore settings) {
			if (highlighting is not T typedHighlighting)
				return null;
			
			var richText = new RichText();
			var highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);
			var colorizer = new CSharpColorizer(richText, _textStyleHighlighterManager, _codeAnnotationsConfiguration, highlighterIdProvider);
			AppendTooltip(typedHighlighting, colorizer);
			return richText;
		}

		protected abstract void AppendTooltip(T highlighting, CSharpColorizer colorizer);

		protected CSharpHighlightingEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory) {
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsConfiguration = codeAnnotationsConfiguration;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}