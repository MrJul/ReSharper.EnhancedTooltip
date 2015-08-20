using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
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

		public new IParameterInfoContext CreateContext(ISolution solution,
			IDocument document,
			int caretOffset,
			int expectedLParenthOffset,
			char invocationChar,
			IContextBoundSettingsStore contextBoundSettingsStore) {

			IParameterInfoContext context = base.CreateContext(
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

	}

}