using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ParameterInfo;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.ReSharper.Psi.CSharp;

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
	public class EnhancedParameterInfoContextFactory : CSParameterInfoContextFactory, IParameterInfoContextFactory {

		public new IParameterInfoContext CreateContext(
			ISolution solution,
			DocumentOffset caretOffset,
			DocumentOffset expectedLParenthOffset,
			char invocationChar,
			IContextBoundSettingsStore settingsStore) {

			IParameterInfoContext context = base.CreateContext(solution, caretOffset, expectedLParenthOffset, invocationChar, settingsStore);
			if (!context.CanEnhance(settingsStore))
				return context;

			return new EnhancedParameterInfoContext(
				context,
				settingsStore,
				solution.GetComponent<HighlighterIdProviderFactory>(),
				solution.GetComponent<ColorizerPresenter>());
		}

	}

}