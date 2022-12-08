using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ParameterInfo;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi.CSharp;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// This is the top of the override chain needed to replace an existing type argument info.
	/// </summary>
	/// <remarks>
	/// The override chain contains the following elements:
	///  - <see cref="IParameterInfoContextFactory"/> (overridden by <see cref="EnhancedTypeArgumentContextFactory"/>)
	///  - <see cref="IParameterInfoContext"/> (overridden by <see cref="EnhancedTypeArgumentContext"/>)
	///  - <see cref="ParameterInfoCandidate"/> (overridden by <see cref="EnhancedTypeArgumentCandidate"/>)
	/// </remarks>
	[ParameterInfoContextFactory(typeof(CSharpLanguage))]
	public class EnhancedTypeArgumentContextFactory : CSTypeArgumentsInfoContextFactory, IParameterInfoContextFactory {

		public new IParameterInfoContext? CreateContext(
			ISolution solution,
			DocumentOffset caretOffset,
			DocumentOffset expectedLParenthOffset,
			char invocationChar,
			IContextBoundSettingsStore settingsStore) {

			IParameterInfoContext? context = base.CreateContext(solution, caretOffset, expectedLParenthOffset, invocationChar, settingsStore);
			if (context is null || !settingsStore.GetValue((Settings.ParameterInfoSettings s) => s.Enabled))
				return context;

			return new EnhancedTypeArgumentContext(
				context,
				settingsStore,
				solution.GetComponent<HighlighterIdProviderFactory>(),
				solution.GetComponent<ColorizerPresenter>());
		}

	}

}