using System.Collections.Generic;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	public class IdentifierContentGroup {

		public List<IdentifierTooltipContent> Identifiers { get; } = new();

		public ArgumentRoleTooltipContent? ArgumentRole { get; set; }

	}

}