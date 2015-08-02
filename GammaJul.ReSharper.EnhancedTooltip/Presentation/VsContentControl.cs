using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Language.Intellisense;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal class VsContentControl : HeaderedContentControl {

		[NotNull]
		protected IReadOnlyCollection<object> VsContents { get; }

		public VsContentControl([NotNull] IReadOnlyCollection<object> vsContents) {
			VsContents = vsContents;

			Style = UIResources.Instance.HeaderedContentControlStyle;
			Focusable = false;
			Header = "VS";
			Content = new ItemsControl {
				Focusable = false,
				ItemsSource = vsContents
			};
		}

	}

	internal class InteractiveVsContentControl : VsContentControl, IInteractiveQuickInfoContent {
		
		public bool KeepQuickInfoOpen
			=> VsContents.OfType<IInteractiveQuickInfoContent>().Any(c => c.KeepQuickInfoOpen);

		public bool IsMouseOverAggregated
			=> VsContents.OfType<IInteractiveQuickInfoContent>().Any(c => c.IsMouseOverAggregated);

		public InteractiveVsContentControl([NotNull] IReadOnlyCollection<object> vsContents)
			: base(vsContents) {
		}

	}

}