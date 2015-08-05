using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using JetBrains.Util.Lazy;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// This is the top of the override chain needed to replace an existing parameter info.
	/// </summary>
	/// <remarks>
	/// The override chain contains the following elements:
	///  - <see cref="IParameterInfoContextFactory"/> (overridden by <see cref="EnhancedParameterInfoContextFactory"/>)
	///  - <see cref="IParameterInfoContext"/> (overridden by <see cref="EnhancedParameterInfoContext"/>)
	///  - <see cref="ParameterInfoCandidate"/> (overridden by <see cref="EnhancedParameterInfoCandidate"/>)
	/// </remarks>
	[ParameterInfoContextFactory(typeof(CSharpLanguage))]
	public class EnhancedParameterInfoContextFactory : IParameterInfoContextFactory {

		[NotNull] [ItemCanBeNull] private readonly Lazy<IParameterInfoContextFactory> _csParameterInfoContextFactory;

		public bool IsIntellisenseEnabled(ISolution solution, IContextBoundSettingsStore contextBoundSettingsStore)
			=> _csParameterInfoContextFactory.Value?.IsIntellisenseEnabled(solution, contextBoundSettingsStore) == true;

		public IParameterInfoContext CreateContext(ISolution solution,
			IDocument document,
			int caretOffset,
			int expectedLParenthOffset,
			char invocationChar,
			IContextBoundSettingsStore contextBoundSettingsStore) {

			IParameterInfoContext context = _csParameterInfoContextFactory.Value?.CreateContext(
				solution, document, caretOffset, expectedLParenthOffset, invocationChar, contextBoundSettingsStore);
			return Enhance(context, solution, contextBoundSettingsStore);
		}

		[CanBeNull]
		private static IParameterInfoContext Enhance([CanBeNull] IParameterInfoContext context, [NotNull] ISolution solution, [NotNull] IContextBoundSettingsStore settings) {
			if (context == null || !settings.GetValue((ParameterInfoSettings s) => s.Enabled))
				return context;
			
			return new EnhancedParameterInfoContext(
				context,
				solution.GetComponent<ColorizerPresenter>(),
				solution.GetComponent<HighlighterIdProviderFactory>(),
				settings);
		}

		public bool ShouldPopup(IDocument document, int caretOffset, char c, ISolution solution, IContextBoundSettingsStore contextBoundSettingsStore)
			=> _csParameterInfoContextFactory.Value?.ShouldPopup(document, caretOffset, c, solution, contextBoundSettingsStore) == true;

		public PsiLanguageType Language
			=> CSharpLanguage.Instance;

		public IEnumerable<char> ImportantChars
			=> _csParameterInfoContextFactory.Value?.ImportantChars ?? EmptyList<char>.InstanceList;

		public EnhancedParameterInfoContextFactory() {
			_csParameterInfoContextFactory =
				Lazy.Of(() => Shell.Instance.GetComponent<ILanguageManager>()
					.GetServices<IParameterInfoContextFactory>(Language)
					.FirstOrDefault(f => f.GetType().Name == "CSParameterInfoContextFactory" /* unfortunately the original factory is internal so we can't use typeof */));
		}

	}

}