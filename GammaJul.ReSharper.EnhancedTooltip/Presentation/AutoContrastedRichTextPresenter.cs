using JetBrains.UI.Avalon.Controls;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// Workaround the base <see cref="RichTextPresenter"/> which doesn't re-render its content
	/// when <see cref="RichTextPresenter.IsAutoContrasted"/> changes (it doesn't work if the text has been set first).
	/// </summary>
	public class AutoContrastedRichTextPresenter : RichTextPresenter {

		public AutoContrastedRichTextPresenter() {
			IsAutoContrasted = true;
		}

	}

}