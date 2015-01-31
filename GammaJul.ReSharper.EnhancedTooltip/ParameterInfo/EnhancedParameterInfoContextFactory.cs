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
using JetBrains.Util;
using JetBrains.Util.Lazy;
#if RS90
using JetBrains.ReSharper.Resources.Shell;
#elif RS82
using JetBrains.Application;
#endif

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

		private readonly Lazy<IParameterInfoContextFactory> _csParameterInfoContextFactory;

		public bool IsIntellisenseEnabled(ISolution solution, IContextBoundSettingsStore contextBoundSettingsStore) {
			IParameterInfoContextFactory factory = _csParameterInfoContextFactory.Value;
			return factory != null && factory.IsIntellisenseEnabled(solution, contextBoundSettingsStore);
		}

		public IParameterInfoContext CreateContext(ISolution solution, IDocument document, int caretOffset, int expectedLParenthOffset, char invocationChar,
			IContextBoundSettingsStore contextBoundSettingsStore) {
			IParameterInfoContextFactory factory = _csParameterInfoContextFactory.Value;
			IParameterInfoContext context = factory != null
				? factory.CreateContext(solution, document, caretOffset, expectedLParenthOffset, invocationChar, contextBoundSettingsStore)
				: null;

			return Enhance(context, solution, contextBoundSettingsStore);
		}

		[CanBeNull]
		private static IParameterInfoContext Enhance([CanBeNull] IParameterInfoContext context, [NotNull] ISolution solution, [NotNull] IContextBoundSettingsStore settings) {
			if (!settings.GetValue((ParameterInfoSettings s) => s.Enabled))
				return context;

			return context == null
				? null
				: new EnhancedParameterInfoContext(context, solution.GetComponent<ColorizerPresenter>(), settings);
		}

		public bool ShouldPopup(IDocument document, int caretOffset, char c, ISolution solution, IContextBoundSettingsStore contextBoundSettingsStore) {
			IParameterInfoContextFactory factory = _csParameterInfoContextFactory.Value;
			return factory != null && factory.ShouldPopup(document, caretOffset, c, solution, contextBoundSettingsStore);
		}

		public PsiLanguageType Language {
			get { return CSharpLanguage.Instance; }
		}

		public IEnumerable<char> ImportantChars {
			get {
				IParameterInfoContextFactory factory = _csParameterInfoContextFactory.Value;
				return factory != null ? factory.ImportantChars : EmptyList<char>.InstanceList;
			}
		}
		
		public EnhancedParameterInfoContextFactory() {
			_csParameterInfoContextFactory =
				Lazy.Of(() => Shell.Instance.GetComponent<ILanguageManager>()
					.GetServices<IParameterInfoContextFactory>(Language)
					.FirstOrDefault(f => f.GetType().Name == "CSParameterInfoContextFactory" /* unfortunately the original factory is internal so we can't use typeof */));
		}

	}

}