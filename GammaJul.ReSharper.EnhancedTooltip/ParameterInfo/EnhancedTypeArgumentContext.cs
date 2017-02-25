using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Part of the override chain needed to replace a parameter info.
	/// <see cref="EnhancedTypeArgumentContextFactory"/> for more information.
	/// </summary>
	public class EnhancedTypeArgumentContext : EnhancedContext<TypeArgumentCandidate> {

		[NotNull] private readonly IContextBoundSettingsStore _settings;
		[NotNull] private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;
		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		
		protected override EnhancedCandidate<TypeArgumentCandidate> Enhance(TypeArgumentCandidate candidate)
			=> new EnhancedTypeArgumentCandidate(candidate, _settings, _highlighterIdProviderFactory, _colorizerPresenter);

		public EnhancedTypeArgumentContext(
			[NotNull] IParameterInfoContext context,
			[NotNull] IContextBoundSettingsStore settings,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory,
			[NotNull] ColorizerPresenter colorizerPresenter)
			: base(context) {
			_settings = settings;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}