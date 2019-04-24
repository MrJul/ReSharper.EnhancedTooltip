using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Util.Colors;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using Color = System.Drawing.Color;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// A component that retrieve <see cref="TextStyle"/>s from either ReSharper's highlighters or Visual Studio colors.
	/// </summary>
	[SolutionComponent]
	public class TextStyleHighlighterManager {

		[NotNull] private readonly Dictionary<string, TextStyle> _vsAttributesByName = new Dictionary<string, TextStyle>();
		[NotNull] private readonly IHighlighterCustomization _highlighterCustomization;
		[NotNull] private readonly Lazy<Optional<IClassificationFormatMapService>> _lazyClassificationFormatMapService;
		[NotNull] private readonly Lazy<Optional<IClassificationTypeRegistryService>> _lazyClassificationTypeRegistryService;

		private TextStyle GetReSharperHighlighterAttributes([NotNull] string highlighterAttributeId)
			=> ToTextStyle(_highlighterCustomization.GetCustomizedRegisteredHighlighterAttributes(highlighterAttributeId));

		private TextStyle GetVsHighlighterAttributes([NotNull] string highlighterAttributeId) {
			lock (_vsAttributesByName)
				return _vsAttributesByName.GetOrCreateValue(highlighterAttributeId, GetVsHighlighterAttributesNoCache);
		}

		private TextStyle GetVsHighlighterAttributesNoCache([NotNull] string highlighterAttributeId) {
			var map = _lazyClassificationFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("text");
			if (map == null)
				return TextStyle.Default;

			IClassificationType classificationType = _lazyClassificationTypeRegistryService.Value.CanBeNull?.GetClassificationType(highlighterAttributeId);
			if (classificationType == null)
				return TextStyle.Default;

			var properties = map.GetTextProperties(classificationType);
			return ToTextStyle(properties);
		}

		private TextStyle GetHighlighterAttributes([NotNull] string highlighterAttributeId)
			=> highlighterAttributeId.StartsWith("ReSharper", StringComparison.Ordinal)
				? GetReSharperHighlighterAttributes(highlighterAttributeId)
				: GetVsHighlighterAttributes(highlighterAttributeId);

		private static TextStyle ToTextStyle([NotNull] HighlighterAttributes attributes) {
			FontStyle fontStyle = attributes.FontStyle != HighlighterAttributes.UNDEFINED_FONT_STYLE ? (FontStyle) attributes.FontStyle : FontStyle.Regular;
			return new TextStyle(fontStyle, attributes.Color, attributes.BackgroundColor);
		}

		private static TextStyle ToTextStyle([CanBeNull] TextFormattingRunProperties properties)
			=> properties != null
				? new TextStyle(GetFontStyle(properties), GetColor(properties))
				: TextStyle.Default;

		private static Color GetColor([NotNull] TextFormattingRunProperties properties)
			=> !properties.ForegroundBrushEmpty && properties.ForegroundBrush is SolidColorBrush solidColorBrush
				? solidColorBrush.Color.ToWinFormsColor()
				: Color.Empty;

		private static FontStyle GetFontStyle([NotNull] TextFormattingRunProperties properties)
			=> properties.BoldEmpty || !properties.Bold ? FontStyle.Regular : FontStyle.Bold;

		public TextStyle GetHighlighterTextStyle([CanBeNull] string highlighterAttributeId)
			=> highlighterAttributeId.IsEmpty()
				? TextStyle.Default
				: GetHighlighterAttributes(highlighterAttributeId);

		private void ResetVsAttributesCache() {
			lock (_vsAttributesByName)
				_vsAttributesByName.Clear();
		}

		public TextStyleHighlighterManager(
			Lifetime lifetime,
			[NotNull] IHighlighterCustomization highlighterCustomization,
			[NotNull] ITextControlSchemeManager textControlSchemeManager,
			[NotNull] Lazy<Optional<IClassificationFormatMapService>> lazyClassificationFormatMapService,
			[NotNull] Lazy<Optional<IClassificationTypeRegistryService>> lazyClassificationTypeRegistryService) {
			_highlighterCustomization = highlighterCustomization;
			_lazyClassificationFormatMapService = lazyClassificationFormatMapService;
			_lazyClassificationTypeRegistryService = lazyClassificationTypeRegistryService;
			textControlSchemeManager.ColorsChanged.Advise(lifetime, ResetVsAttributesCache);
		}

	}

}