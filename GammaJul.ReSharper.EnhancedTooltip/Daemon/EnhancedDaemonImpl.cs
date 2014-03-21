using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Tasks;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.GeneratedCode;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;

namespace GammaJul.ReSharper.EnhancedTooltip.Daemon {

	/// <summary>
	/// This is the top of the override chain needed to replace an existing tooltip.
	/// </summary>
	/// <remarks>
	/// The override chain contains the following elements:
	///  - <see cref="Daemon"/> (overridden by <see cref="EnhancedDaemonImpl"/>)
	///  - <see cref="VisibleDocumentDaemonProcess"/> (overridden by <see cref="EnhancedVisibleDocumentDaemonProcess"/>)
	///  - <see cref="IDocumentMarkupManager"/> (overriden by <see cref="EnhancedDocumentMarkupManager"/>)
	///  - <see cref="IDocumentMarkup"/> (overriden by <see cref="EnhancedDocumentMarkup"/>)
	///  - <see cref="IHighlighterTooltipProvider"/> (overriden by <see cref="EnhancedHighlighterTooltipProvider"/>)
	/// Ideally, a better extensibility point should be used. I haven't found any though.
	/// </remarks>
	[SolutionComponent]
	public class EnhancedDaemonImpl : DaemonImpl {

		private readonly IShellLocks _locks;
		private readonly ISolution _solution;
		private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		private readonly ColorizerPresenter _colorizerPresenter;

		protected override VisibleDocumentDaemonProcess CreateDaemonForDocumentImpl(IPsiSourceFile sourceFile) {
			return new EnhancedVisibleDocumentDaemonProcess(sourceFile, myDocumentMarkupManager, myTextControlManager, myDocumentManager,
				mySolutionAnalysisService, myHighlightingSettingsManager, _locks, mySolutionDocumentTransactionManager, myLifetime,
				_solution, _declaredElementDescriptionPresenter, _colorizerPresenter);
		}

		public EnhancedDaemonImpl(ISolution solution, Lifetime lifetime, IPsiServices psiServices, DocumentManager documentManager, IShellLocks locks,
			ITextControlManager textControlManager, ChangeManager changeManager, DaemonStageManager daemonStageManager,
			HighlightingSettingsManager highlightingSettingsManager, DaemonExcludedFilesManager excludedFilesManager,
			SolutionAnalysisService solutionAnalysisService, IDocumentMarkupManager documentMarkupManager, AsyncCommitService asyncCommitService,
			SolutionAnalysisIndicator solutionAnalysisIndicator, ISettingsStore settingsStore, IDaemonThread daemonThread,
			ISolutionLoadTasksScheduler scheduler, DaemonAutoDisableStrategy daemonAutoDisableStrategy,
			SolutionDocumentTransactionManager solutionDocumentTransactionManager, DocumentToProjectFileMappingStorage documentStorage,
			IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter, ColorizerPresenter colorizerPresenter)
			: base(solution, lifetime, psiServices, documentManager, locks, textControlManager, changeManager, daemonStageManager, highlightingSettingsManager,
				excludedFilesManager, solutionAnalysisService, documentMarkupManager, asyncCommitService, solutionAnalysisIndicator, settingsStore, daemonThread,
				scheduler, daemonAutoDisableStrategy, solutionDocumentTransactionManager, documentStorage) {
			_solution = solution;
			_locks = locks;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}