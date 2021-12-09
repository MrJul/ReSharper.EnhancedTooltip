using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	public abstract class EnhancedContext<TCandidate> : IParameterInfoContext
	where TCandidate : class, ICandidate {

		private readonly IParameterInfoContext _context;
		private ICandidate[]? _candidates;
		private ICandidate? _defaultCandidate;

		private ICandidate[] Enhance(ICandidate[] candidates) {
			int length = candidates.Length;
			var wrappedCandidates = new ICandidate[length];
			for (int i = 0; i < length; i++)
				wrappedCandidates[i] = Enhance(candidates[i]);
			return wrappedCandidates;
		}

		private ICandidate Enhance(ICandidate candidate)
			=> candidate is TCandidate typedCandidate
			? Enhance(typedCandidate)
			: candidate;

		protected abstract EnhancedCandidate<TCandidate> Enhance(TCandidate candidate);

		public int Argument
			=> _context.Argument;

		public ICandidate[] Candidates
			=> _candidates ??= Enhance(_context.Candidates);

		public ICandidate? DefaultCandidate
			=> _defaultCandidate ??= FindDefaultCandidate();

		private ICandidate? FindDefaultCandidate() {
			if (_context.DefaultCandidate is not { } candidateToFind)
				return null;

			foreach (ICandidate candidate in Candidates) {
				if (ReferenceEquals(candidateToFind, candidate))
					return candidate;

				if (candidate is EnhancedCandidate<TCandidate> enhancedCandidate
				&& ReferenceEquals(enhancedCandidate.UnderlyingCandidate, candidateToFind))
					return enhancedCandidate;
			}
			
			return null;
		}

		public string[] NamedArguments {
			get => _context.NamedArguments;
			set => _context.NamedArguments = value;
		}

		public Type ParameterListNodeType
			=> _context.ParameterListNodeType;

		public ICollection<Type> ParameterNodeTypes
			=> _context.ParameterNodeTypes;

		public TextRange Range
			=> _context.Range;

		protected EnhancedContext(IParameterInfoContext context) {
			_context = context;
		}

	}

}