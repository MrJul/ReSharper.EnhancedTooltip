using System;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent(Instantiation.ContainerAsyncAnyThreadSafe)]
	internal sealed class AbstractInheritedMemberIsNotImplementedErrorEnhancer : CSharpHighlightingEnhancer<AbstractInheritedMemberIsNotImplementedError> {

		protected override void AppendTooltip(AbstractInheritedMemberIsNotImplementedError highlighting, CSharpColorizer colorizer) {
			TypeMemberInstance[] members = highlighting.NotImplementedMembers;

			for (int i = 0; i < members.Length; ++i) {
				if (i > 0)
					colorizer.AppendPlainText(Environment.NewLine);

				colorizer.AppendPlainText("Abstract inherited member '");
				TypeMemberInstance member = members[i];
				colorizer.AppendDeclaredElement(member.Element, member.Substitution, PresenterOptions.QualifiedMember, highlighting.Declaration);
				colorizer.AppendPlainText("' is not implemented");
			}
		}

		public AbstractInheritedMemberIsNotImplementedErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}