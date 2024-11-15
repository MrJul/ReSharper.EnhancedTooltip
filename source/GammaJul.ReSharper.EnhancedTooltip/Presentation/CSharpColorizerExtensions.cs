using GammaJul.ReSharper.EnhancedTooltip.Psi;
using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static class CSharpColorizerExtensions {

		public static void AppendPlainText(this CSharpColorizer colorizer, string? text)
			=> colorizer.AppendText(text, null);

    public static void AppendRichText(this CSharpColorizer colorizer, RichText text)
      => colorizer.AppendRichText(text);

		public static void AppendKeyword(this CSharpColorizer colorizer, string? keyword)
			=> colorizer.AppendText(keyword, colorizer.HighlighterIdProvider.Keyword);

		public static void AppendOperator(this CSharpColorizer colorizer, string? @operator)
			=> colorizer.AppendText(@operator, colorizer.HighlighterIdProvider.Operator);

		public static void AppendNamespaceName(this CSharpColorizer colorizer, string? className)
			=> colorizer.AppendText(className, colorizer.HighlighterIdProvider.Namespace);

		public static void AppendClassName(this CSharpColorizer colorizer, string? className)
			=> colorizer.AppendText(className, colorizer.HighlighterIdProvider.Class);

		public static void AppendInterfaceName(this CSharpColorizer colorizer, string? interfaceName)
			=> colorizer.AppendText(interfaceName, colorizer.HighlighterIdProvider.Interface);

		public static void AppendMethodName(this CSharpColorizer colorizer, string? methodName)
			=> colorizer.AppendText(methodName, colorizer.HighlighterIdProvider.Method);

		public static void AppendAccessorName(this CSharpColorizer colorizer, string? accessorName)
			=> colorizer.AppendText(accessorName, colorizer.HighlighterIdProvider.Accessor);

		public static void AppendElementKind(this CSharpColorizer colorizer, IDeclaredElement? element)
			=> colorizer.AppendText(element.GetElementKindString(false, false, false, false, false), null);

    public static void AppendParameterName(this CSharpColorizer colorizer, string? parameterName)
      => colorizer.AppendText(parameterName, colorizer.HighlighterIdProvider.Parameter);

		public static void AppendCandidates(this CSharpColorizer colorizer, IReference? reference) {
			if (reference is null)
				return;

			IResolveResult resolveResult = reference.Resolve().Result;
			IList<IDeclaredElement> candidates = resolveResult.Candidates;
			IList<ISubstitution> substitutions = resolveResult.CandidateSubstitutions;

			for (int i = 0; i < candidates.Count; ++i) {
				colorizer.AppendPlainText(Environment.NewLine);
				colorizer.AppendPlainText("  ");
				colorizer.AppendDeclaredElement(candidates[i].EliminateDelegateInvokeMethod(), substitutions[i], PresenterOptions.FullWithoutParameterNames, reference.GetTreeNode());
			}
		}

		public static void AppendParameterKind(this CSharpColorizer colorizer, ParameterKind parameterKind) {
			switch (parameterKind) {
				case ParameterKind.VALUE:
					colorizer.AppendPlainText("value");
					return;
				case ParameterKind.REFERENCE:
					colorizer.AppendKeyword("ref");
					return;
				case ParameterKind.OUTPUT:
					colorizer.AppendKeyword("out");
					return;
				case ParameterKind.INPUT:
					colorizer.AppendKeyword("in");
					return;
				default:
					colorizer.AppendPlainText("value");
					return;
			}
		}

		public static void AppendAccessorKind(this CSharpColorizer colorizer, AccessorKind accessorKind) {
			switch (accessorKind) {
				case AccessorKind.ADDER:
					colorizer.AppendKeyword("add");
					return;
				case AccessorKind.REMOVER:
					colorizer.AppendKeyword("remove");
					return;
				case AccessorKind.GETTER:
					colorizer.AppendKeyword("get");
					return;
				case AccessorKind.SETTER:
					colorizer.AppendKeyword("set");
					return;
				case AccessorKind.RAISER:
					colorizer.AppendPlainText("fire");
					return;
				case AccessorKind.UNKNOWN:
					colorizer.AppendPlainText("unknown");
					return;
			}
		}

	}

}