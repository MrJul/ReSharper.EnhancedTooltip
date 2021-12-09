using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Part of the override chain needed to replace a parameter info.
	/// <see cref="EnhancedParameterInfoContextFactory"/> for more information.
	/// </summary>
	public class EnhancedParameterInfoContext : EnhancedContext<ParameterInfoCandidate> {

		private readonly IContextBoundSettingsStore _settings;
		private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;
		private readonly ColorizerPresenter _colorizerPresenter;
		
		protected override EnhancedCandidate<ParameterInfoCandidate> Enhance(ParameterInfoCandidate candidate)
			=> new EnhancedParameterInfoCandidate(candidate, _settings, _highlighterIdProviderFactory, _colorizerPresenter);

		public EnhancedParameterInfoContext(
			IParameterInfoContext context,
			IContextBoundSettingsStore settings,
			HighlighterIdProviderFactory highlighterIdProviderFactory,
			ColorizerPresenter colorizerPresenter)
			: base(context) {
			_settings = settings;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}