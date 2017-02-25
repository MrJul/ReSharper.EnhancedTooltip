using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ParameterInfo;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.ParameterInfo {

	public abstract class EnhancedContext<TCandidate> : IParameterInfoContext
	where TCandidate : class, ICandidate {

		[NotNull] private readonly IParameterInfoContext _context;
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
			var typedCandidate = candidate as TCandidate;
			return typedCandidate != null
				? Enhance(typedCandidate)
				: candidate;
		}

		[NotNull]
		protected abstract EnhancedCandidate<TCandidate> Enhance([NotNull] TCandidate candidate);

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

				var enhancedCandidate = candidate as EnhancedCandidate<TCandidate>;
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

		protected EnhancedContext([NotNull] IParameterInfoContext context) {
			_context = context;
		}

	}

}