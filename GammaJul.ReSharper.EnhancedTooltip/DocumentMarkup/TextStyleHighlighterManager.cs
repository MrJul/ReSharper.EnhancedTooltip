using System;
using System.Collections.Generic;
using System.Drawing;
using EnvDTE;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;
using JetBrains.Util;
#if RS90
using JetBrains.VsIntegration.TextControl;
#elif RS82
using JetBrains.VsIntegration;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// A component that retrieve <see cref="HighlighterAttributes"/> from either ReSharper's highlighters or Visual Studio colors.
	/// </summary>
	[SolutionComponent]
	public class TextStyleHighlighterManager {

		private readonly Dictionary<string, HighlighterAttributes> _vsAttributesByName = new Dictionary<string, HighlighterAttributes>();
		private readonly IHighlighterCustomization _highlighterCustomization;
		private readonly DTE _dte;

		[NotNull]
		private HighlighterAttributes GetReSharperHighlighterAttributes([NotNull] string highlighterAttributeId) {
			return _highlighterCustomization.GetCustomizedRegisteredHighlighterAttributes(highlighterAttributeId);
		}

		[NotNull]
		private HighlighterAttributes GetVsHighlighterAttributes([NotNull] string highlighterAttributeId) {
			lock (_vsAttributesByName)
				return _vsAttributesByName.GetOrCreateValue(highlighterAttributeId, GetVsHighlighterAttributesNoCache);
		}

		[NotNull]
		private HighlighterAttributes GetVsHighlighterAttributesNoCache([NotNull] string highlighterAttributeId) {
			ColorableItems colorableItems = VsColorUtilities.TryGetColorableItemsByName(highlighterAttributeId, _dte);
			if (colorableItems == null)
				return HighlighterAttributes.UNDEFINED;
			
			Color foregroundColor = ColorTranslator.FromOle((int) colorableItems.Foreground);
			var attributes = new HighlighterAttributes(foregroundColor);
			if (colorableItems.Bold)
				attributes = attributes.Merge(new HighlighterAttributes(FontStyle.Bold));
			return attributes;
		}

		[NotNull]
		private HighlighterAttributes GetHighlighterAttributes([NotNull] string highlighterAttributeId) {
			return highlighterAttributeId.StartsWith("ReSharper", StringComparison.Ordinal)
				? GetReSharperHighlighterAttributes(highlighterAttributeId)
				: GetVsHighlighterAttributes(highlighterAttributeId);
		}

		private static TextStyle ToTextStyle([NotNull] HighlighterAttributes attributes) {
			FontStyle fontStyle = attributes.FontStyle != HighlighterAttributes.UNDEFINED_FONT_STYLE ? (FontStyle) attributes.FontStyle : FontStyle.Regular;
			return new TextStyle(fontStyle, attributes.Color, attributes.BackgroundColor);
		}

		public TextStyle GetHighlighterTextStyle([CanBeNull] string highlighterAttributeId) {
			if (highlighterAttributeId.IsEmpty())
				return TextStyle.Default;
			return ToTextStyle(GetHighlighterAttributes(highlighterAttributeId));
		}

		private void ResetVsAttributesCache() {
			lock (_vsAttributesByName)
				_vsAttributesByName.Clear();
		}

		public TextStyleHighlighterManager([NotNull] Lifetime lifetime, [NotNull] IHighlighterCustomization highlighterCustomization,
			[NotNull] DTE dte, [NotNull] DefaultTextControlSchemeManager textControlSchemeManager) {
			_highlighterCustomization = highlighterCustomization;
			_dte = dte;
			textControlSchemeManager.ColorsChanged.Advise(lifetime, ResetVsAttributesCache);
		}

	}

}