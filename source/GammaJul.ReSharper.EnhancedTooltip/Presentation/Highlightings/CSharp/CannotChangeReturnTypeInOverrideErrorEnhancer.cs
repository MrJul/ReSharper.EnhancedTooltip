using System;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Resolve;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation.Highlightings.CSharp {

	[SolutionComponent]
	internal sealed class CannotChangeReturnTypeInOverrideErrorEnhancer : CSharpHighlightingEnhancer<CannotChangeReturnTypeInOverrideError> {

		protected override void AppendTooltip(CannotChangeReturnTypeInOverrideError highlighting, CSharpColorizer colorizer) {
			if (highlighting.TypeUsageMismatch.HasShortDescription) {
				colorizer.AppendPlainText(highlighting.TypeUsageMismatch.GetDescription());
				colorizer.AppendPlainText(Environment.NewLine);
				colorizer.AppendPlainText(Environment.NewLine);
			}
			colorizer.AppendPlainText("Cannot change return type when overriding ");
			colorizer.AppendElementKind(highlighting.OverriddenMember);
			colorizer.AppendPlainText(" '");
			colorizer.AppendDeclaredElement(highlighting.OverriddenMember, EmptySubstitution.INSTANCE, PresenterOptions.QualifiedMember, highlighting.Declaration);
			colorizer.AppendPlainText("'");
		}
		
		public CannotChangeReturnTypeInOverrideErrorEnhancer(
			TextStyleHighlighterManager textStyleHighlighterManager,
			CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			HighlighterIdProviderFactory highlighterIdProviderFactory)
			: base(textStyleHighlighterManager, codeAnnotationsConfiguration, highlighterIdProviderFactory) {
		}

	}

}