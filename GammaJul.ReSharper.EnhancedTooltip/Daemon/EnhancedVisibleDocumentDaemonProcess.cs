using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;

namespace GammaJul.ReSharper.EnhancedTooltip.Daemon {

	/// <summary>
	/// Part of the override chain needed to replace a tooltip.
	/// <see cref="EnhancedDaemonImpl"/> for more information.
	/// </summary>
	public class EnhancedVisibleDocumentDaemonProcess : VisibleDocumentDaemonProcess {

		[NotNull]
		private static EnhancedDocumentMarkupManager CreateEnhancedDocumentMarkupManager([NotNull] IDocumentMarkupManager documentMarkupManager,
			[NotNull] ISolution solution, [NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter, [NotNull] IHighlighterCustomization highlighterCustomization,
			[NotNull] ColorizerPresenter colorizerPresenter) {
			return new EnhancedDocumentMarkupManager(documentMarkupManager, solution, declaredElementDescriptionPresenter, highlighterCustomization, colorizerPresenter);
		}

		public EnhancedVisibleDocumentDaemonProcess(IPsiSourceFile sourceFile, IDocumentMarkupManager documentMarkupManager,
			ITextControlManager textControlManager, DocumentManager documentManager, SolutionAnalysisService solutionAnalysisService,
			HighlightingSettingsManager highlightingSettingsManager, IShellLocks locks, SolutionDocumentTransactionManager documentTransactionManager,
			Lifetime lifetime, ISolution solution, IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter,
			IHighlighterCustomization highlighterCustomization, ColorizerPresenter colorizerPresenter)
			: base(sourceFile, CreateEnhancedDocumentMarkupManager(documentMarkupManager, solution, declaredElementDescriptionPresenter, highlighterCustomization, colorizerPresenter),
				textControlManager, documentManager, solutionAnalysisService, highlightingSettingsManager, locks, documentTransactionManager, lifetime) {
		}

	}

}