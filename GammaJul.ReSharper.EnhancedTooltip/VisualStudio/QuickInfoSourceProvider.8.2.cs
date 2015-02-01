using JetBrains.Annotations;
using Microsoft.VisualStudio.Editor;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {

	public partial class QuickInfoSourceProvider {
		
		public QuickInfoSourceProvider([CanBeNull] IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService) {
			VsEditorAdaptersFactoryService = vsEditorAdaptersFactoryService;
		}
		 

	}

}