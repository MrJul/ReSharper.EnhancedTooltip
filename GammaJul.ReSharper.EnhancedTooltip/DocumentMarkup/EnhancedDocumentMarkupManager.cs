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
	public class EnhancedDocumentMarkupManager : IDocumentMarkupManager {

		private static readonly Key<EnhancedDocumentMarkup> _enhancedDocumentMarkUpKey = new Key<EnhancedDocumentMarkup>("EnhancedMarkup");

		private readonly IDocumentMarkupManager _underlyingManager;
		private readonly ISolution _solution;
		private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		private readonly ColorizerPresenter _colorizerPresenter;

		public IDocumentMarkup GetMarkupModel(IDocument document) {
			EnhancedDocumentMarkup documentMarkup = document.GetData(_enhancedDocumentMarkUpKey);
			if (documentMarkup == null) {
				documentMarkup = new EnhancedDocumentMarkup(_underlyingManager.GetMarkupModel(document),
					_solution, _declaredElementDescriptionPresenter, _colorizerPresenter);
				document.PutData(_enhancedDocumentMarkUpKey, documentMarkup);
			}
			return documentMarkup;
		}

		public EnhancedDocumentMarkupManager([NotNull] IDocumentMarkupManager underlyingManager, [NotNull] ISolution solution,
			[NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter, [NotNull] ColorizerPresenter colorizerPresenter) {
			_underlyingManager = underlyingManager;
			_solution = solution;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}