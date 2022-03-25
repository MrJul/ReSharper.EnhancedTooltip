using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Util.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using Color = System.Drawing.Color;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// A component that retrieve <see cref="TextStyle"/>s from either ReSharper's highlighters or Visual Studio colors.
	/// </summary>
	[SolutionComponent]
	public class TextStyleHighlighterManager {

		private readonly Dictionary<string, TextStyle> _vsAttributesByName = new();
		private readonly IHighlighterCustomization _highlighterCustomization;
		private readonly Lazy<Optional<IClassificationFormatMapService>> _lazyClassificationFormatMapService;
		private readonly Lazy<Optional<IClassificationTypeRegistryService>> _lazyClassificationTypeRegistryService;

		private TextStyle GetReSharperHighlighterAttributes(string highlighterAttributeId)
			=> ToTextStyle(_highlighterCustomization.GetCustomizedRegisteredHighlighterAttributes(highlighterAttributeId));

		private TextStyle GetVsHighlighterAttributes(string highlighterAttributeId) {
			lock (_vsAttributesByName)
				return _vsAttributesByName.GetOrCreateValue(highlighterAttributeId, GetVsHighlighterAttributesNoCache);
		}

		private TextStyle GetVsHighlighterAttributesNoCache(string highlighterAttributeId) {
			if (_lazyClassificationFormatMapService.Value.CanBeNull?.GetClassificationFormatMap("text") is { } map
			&& _lazyClassificationTypeRegistryService.Value.CanBeNull?.GetClassificationType(highlighterAttributeId) is { } classificationType) {
				var properties = map.GetTextProperties(classificationType);
				return ToTextStyle(properties);
			}

			return TextStyle.Default;
		}

		private TextStyle GetHighlighterAttributes(string highlighterAttributeId)
			=> highlighterAttributeId.StartsWith("ReSharper", StringComparison.Ordinal)
				? GetReSharperHighlighterAttributes(highlighterAttributeId)
				: GetVsHighlighterAttributes(highlighterAttributeId);

		private static TextStyle ToTextStyle(CustomizedHighlighterAttributes attributes) {
			return new TextStyle(attributes.FontStyle, attributes.Color, attributes.BackgroundColor);
		}

		private static TextStyle ToTextStyle(TextFormattingRunProperties? properties)
			=> properties is not null
				? new TextStyle(GetFontStyle(properties), GetColor(properties))
				: TextStyle.Default;

		private static JetRgbaColor GetColor(TextFormattingRunProperties properties)
			=> !properties.ForegroundBrushEmpty && properties.ForegroundBrush is SolidColorBrush solidColorBrush
				? solidColorBrush.Color.ToJetRgbaColor()
				: Color.Empty.ToJetRgbaColor();

		private static JetFontStyles GetFontStyle(TextFormattingRunProperties properties)
			=> properties.BoldEmpty || !properties.Bold ? JetFontStyles.Regular : JetFontStyles.Bold;

		public TextStyle GetHighlighterTextStyle(string? highlighterAttributeId)
			=> highlighterAttributeId.IsEmpty()
				? TextStyle.Default
				: GetHighlighterAttributes(highlighterAttributeId!);

		private void ResetVsAttributesCache() {
			lock (_vsAttributesByName)
				_vsAttributesByName.Clear();
		}

		public TextStyleHighlighterManager(
			Lifetime lifetime,
			IHighlighterCustomization highlighterCustomization,
			ITextControlSchemeManager textControlSchemeManager,
			Lazy<Optional<IClassificationFormatMapService>> lazyClassificationFormatMapService,
			Lazy<Optional<IClassificationTypeRegistryService>> lazyClassificationTypeRegistryService) {
			_highlighterCustomization = highlighterCustomization;
			_lazyClassificationFormatMapService = lazyClassificationFormatMapService;
			_lazyClassificationTypeRegistryService = lazyClassificationTypeRegistryService;
			textControlSchemeManager.ColorsChanged.Advise(lifetime, ResetVsAttributesCache);
		}

	}

}