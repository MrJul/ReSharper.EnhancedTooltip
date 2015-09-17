using System.Collections.Generic;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IdentifierContentGroup {

		[NotNull]
		public List<IdentifierTooltipContent> Identifiers { get; } = new List<IdentifierTooltipContent>();

		[CanBeNull]
		public ArgumentRoleTooltipContent ArgumentRole { get; set; }

	}

}