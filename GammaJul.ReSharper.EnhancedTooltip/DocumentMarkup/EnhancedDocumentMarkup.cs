using System;
using System.Collections.Generic;
using GammaJul.ReSharper.EnhancedTooltip.Daemon;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// Part of the override chain needed to replace a tooltip.
	/// <see cref="EnhancedDaemonImpl"/> for more information.
	/// </summary>
	public class EnhancedDocumentMarkup : IDocumentMarkupImpl {

		private readonly IDocumentMarkup _underlyingMarkup;
		private readonly ISolution _solution;

		private readonly Dictionary<IHighlighterTooltipProvider, EnhancedHighlighterTooltipProvider> _highlighterTooltipProviders
			= new Dictionary<IHighlighterTooltipProvider, EnhancedHighlighterTooltipProvider>();
		private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		private readonly ColorizerPresenter _colorizerPresenter;

		[CanBeNull]
		private EnhancedHighlighterTooltipProvider Enhance([CanBeNull] IHighlighterTooltipProvider provider) {
			if (provider == null)
				return null;

			EnhancedHighlighterTooltipProvider enhancedProvider;
			if (!_highlighterTooltipProviders.TryGetValue(provider, out enhancedProvider)) {
				enhancedProvider = new EnhancedHighlighterTooltipProvider(provider, _solution, _declaredElementDescriptionPresenter, _colorizerPresenter);
				_highlighterTooltipProviders[provider] = enhancedProvider;
			}
			return enhancedProvider;
		}

		public IHighlighter AddHighlighter(Key key, TextRange range, AreaType areaType, int layer, HighlighterAttributes attributes,
			ErrorStripeAttributes errorStripeAttributes, IHighlighterTooltipProvider tooltip) {
			return _underlyingMarkup.AddHighlighter(key, range, areaType, layer, attributes, errorStripeAttributes, Enhance(tooltip));
		}

		public IHighlighter AddHighlighter(Key key, TextRange range, AreaType areaType, int layer, string attributeId, ErrorStripeAttributes errorStripeAttributes,
			IHighlighterTooltipProvider tooltip, IGutterMarkInfo gutterMarkInfo = null) {
			return _underlyingMarkup.AddHighlighter(key, range, areaType, layer, attributeId, errorStripeAttributes, Enhance(tooltip), gutterMarkInfo);
		}

		public IDisposable BatchChangeCookie() {
			return _underlyingMarkup.BatchChangeCookie();
		}

		public event Action<DocumentMarkupModifiedEventArgs> Changed {
			add { _underlyingMarkup.Changed += value; }
			remove { _underlyingMarkup.Changed -= value; }
		}

		public HihglighterContext Context {
			get { return _underlyingMarkup.Context; }
		}

		public void Dispose() {
			_underlyingMarkup.Dispose();
		}

		public IDocument Document {
			get { return _underlyingMarkup.Document; }
		}

		public IEnumerable<IHighlighter> GetHighlightersEnumerable() {
			return _underlyingMarkup.GetHighlightersEnumerable();
		}

		public IEnumerable<IHighlighter> GetHighlightersEnumerable(Key key) {
			return _underlyingMarkup.GetHighlightersEnumerable(key);
		}

		public IEnumerable<IHighlighter> GetHighlightersOver(TextRange textRange) {
			return _underlyingMarkup.GetHighlightersOver(textRange);
		}

		public bool RemoveHighlighter(IHighlighter highlighter) {
			return _underlyingMarkup.RemoveHighlighter(highlighter);
		}

		public void FireHighlighterModified(IHighlighter highlighter) {
			var underlyingDocumentMarkupImpl = _underlyingMarkup as IDocumentMarkupImpl;
			if (underlyingDocumentMarkupImpl != null)
				underlyingDocumentMarkupImpl.FireHighlighterModified(highlighter);
		}

		public EnhancedDocumentMarkup([NotNull] IDocumentMarkup underlyingMarkup, [NotNull] ISolution solution,
			[NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter, [NotNull] ColorizerPresenter colorizerPresenter) {
			_underlyingMarkup = underlyingMarkup;
			_solution = solution;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}