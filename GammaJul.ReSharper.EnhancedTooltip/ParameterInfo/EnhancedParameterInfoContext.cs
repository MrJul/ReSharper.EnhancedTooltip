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

		private readonly IParameterInfoContext _context;
		private readonly ColorizerPresenter _colorizerPresenter;
		private readonly IContextBoundSettingsStore _settings;
		private ICandidate[] _candidates;

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

		public int Argument {
			get { return _context.Argument; }
		}

		public ICandidate[] Candidates {
			get { return _candidates ?? (_candidates = Enhance(_context.Candidates)); }
		}

		public ICandidate DefaultCandidate {
			get { return _context.DefaultCandidate; }
		}

		public string[] NamedArguments {
			get { return _context.NamedArguments; }
			set { _context.NamedArguments = value; }
		}

		public Type ParameterListNodeType {
			get { return _context.ParameterListNodeType; }
		}

		public ICollection<Type> ParameterNodeTypes {
			get { return _context.ParameterNodeTypes; }
		}

		public TextRange Range {
			get { return _context.Range; }
		}

		public EnhancedParameterInfoContext([NotNull] IParameterInfoContext context, [NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IContextBoundSettingsStore settings) {
			_context = context;
			_colorizerPresenter = colorizerPresenter;
			_settings = settings;
		}

	}

}