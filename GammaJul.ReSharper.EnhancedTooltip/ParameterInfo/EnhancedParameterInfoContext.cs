using System;
using System.Collections.Generic;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	/// <summary>
	/// Part of the override chain needed to replace a parameter info.
	/// <see cref="EnhancedParameterInfoContextFactory"/> for more information.
	/// </summary>
	public class EnhancedParameterInfoContext : IParameterInfoContext {

		[NotNull] private readonly IParameterInfoContext _context;
		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		[NotNull] private readonly IContextBoundSettingsStore _settings;
		[CanBeNull] private ICandidate[] _candidates;
		[CanBeNull] private ICandidate _defaultCandidate;

		[NotNull]
		private ICandidate[] Enhance([NotNull] ICandidate[] candidates) {
			int length = candidates.Length;
			var wrappedCandidates = new ICandidate[length];
			for (int i = 0; i < length; i++)
				wrappedCandidates[i] = Enhance(candidates[i]);
			return wrappedCandidates;
		}

		[NotNull]
		private ICandidate Enhance([NotNull] ICandidate candidate) {
			var typedCandidate = candidate as ParameterInfoCandidate;
			return typedCandidate != null
				? new EnhancedParameterInfoCandidate(typedCandidate, _colorizerPresenter, _settings)
				: candidate;
		}

		public int Argument
			=> _context.Argument;

		public ICandidate[] Candidates
			=> _candidates ?? (_candidates = Enhance(_context.Candidates));

		public ICandidate DefaultCandidate
			=> _defaultCandidate ?? (_defaultCandidate = FindDefaultCandidate());

		[CanBeNull]
		private ICandidate FindDefaultCandidate() {
			ICandidate candidateToFind = _context.DefaultCandidate;
			if (candidateToFind == null)
				return null;

			foreach (ICandidate candidate in Candidates) {
				if (ReferenceEquals(candidateToFind, candidate))
					return candidate;

				var enhancedCandidate = candidate as EnhancedParameterInfoCandidate;
				if (enhancedCandidate != null && ReferenceEquals(enhancedCandidate.UnderlyingCandidate, candidateToFind))
					return enhancedCandidate;
			}
			
			return null;
		}

		public string[] NamedArguments {
			get { return _context.NamedArguments; }
			set { _context.NamedArguments = value; }
		}

		public Type ParameterListNodeType
			=> _context.ParameterListNodeType;

		public ICollection<Type> ParameterNodeTypes
			=> _context.ParameterNodeTypes;

		public TextRange Range
			=> _context.Range;

		public EnhancedParameterInfoContext([NotNull] IParameterInfoContext context, [NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IContextBoundSettingsStore settings) {
			_context = context;
			_colorizerPresenter = colorizerPresenter;
			_settings = settings;
		}

	}

}