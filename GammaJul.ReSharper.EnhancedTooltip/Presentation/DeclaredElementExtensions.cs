using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree.Query;
using JetBrains.ReSharper.Psi.CSharp.Util;
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	internal static partial class DeclaredElementExtensions {

		[CanBeNull]
		public static string GetHighlightingAttributeId([CanBeNull] this IDeclaredElement declaredElement, bool useReSharperColors) {
			if (declaredElement is IFunction) {
				
				var method = declaredElement as IMethod;
				if (method != null) {
					if (!useReSharperColors)
						return VsHighlightingAttributeIds.Identifier;
					if (method.IsPredefined)
						return null;
					return method.IsExtensionMethod
						? HighlightingAttributeIds.EXTENSION_METHOD_IDENTIFIER_ATTRIBUTE
						: HighlightingAttributeIds.METHOD_IDENTIFIER_ATTRIBUTE;
				}

				var signOperator = declaredElement as ISignOperator;
				if (signOperator != null) {
					return signOperator.IsPredefined || !useReSharperColors
						? VsHighlightingAttributeIds.Operator
						: HighlightingAttributeIds.OPERATOR_IDENTIFIER_ATTRIBUTE;
				}

				var constructor = declaredElement as IConstructor;
				if (constructor != null)
					return constructor.GetContainingType().GetHighlightingAttributeId(useReSharperColors);

			}

			var field = declaredElement as IField;
			if (field != null) {
				if (!useReSharperColors)
					return VsHighlightingAttributeIds.Identifier;
				return field.IsField
					? HighlightingAttributeIds.FIELD_IDENTIFIER_ATTRIBUTE
					: HighlightingAttributeIds.CONSTANT_IDENTIFIER_ATTRIBUTE;
			}

			var typeElement = declaredElement as ITypeElement;
			if (typeElement != null)
				return typeElement.GetHighlightingAttributeId(useReSharperColors);

			if (!useReSharperColors)
				return VsHighlightingAttributeIds.Identifier;

			if (CSharpDeclaredElementUtil.IsProperty(declaredElement) || CSharpDeclaredElementUtil.IsIndexedProperty(declaredElement))
				return HighlightingAttributeIds.FIELD_IDENTIFIER_ATTRIBUTE;

			if (declaredElement is IEvent)
				return HighlightingAttributeIds.EVENT_IDENTIFIER_ATTRIBUTE;

			if (declaredElement is INamespace)
				return HighlightingAttributeIds.NAMESPACE_IDENTIFIER_ATTRIBUTE;

			var parameter = declaredElement as IParameter;
			if (parameter != null)
				return parameter.IsValueVariable ? VsHighlightingAttributeIds.Identifier : HighlightingAttributeIds.PARAMETER_IDENTIFIER_ATTRIBUTE;

			var localVariable = declaredElement as ILocalVariable;
			if (localVariable != null) {
				return localVariable.IsConstant
					? HighlightingAttributeIds.CONSTANT_IDENTIFIER_ATTRIBUTE
					: HighlightingAttributeIds.LOCAL_VARIABLE_IDENTIFIER_ATTRIBUTE;
			}

			if (declaredElement is ICSharpAnonymousTypeProperty) {
				return declaredElement is IQueryAnonymousTypeProperty
					? HighlightingAttributeIds.LOCAL_VARIABLE_IDENTIFIER_ATTRIBUTE
					: HighlightingAttributeIds.FIELD_IDENTIFIER_ATTRIBUTE;
			}

			if (declaredElement is IPathDeclaredElement)
				return HighlightingAttributeIds.PATH_IDENTIFIER_ATTRIBUTE;

			return null;
		}

		public static string GetHighlightingAttributeId([CanBeNull] this ITypeElement typeElement, bool useReSharperColors) {
			if (typeElement == null)
				return null;
			if (useReSharperColors)
				return HighlightingAttributeIds.GetHighlightAttributeForTypeElement(typeElement);
			return VsHighlightingAttributeIds.GetForTypeElement(typeElement);
		}

	}

}