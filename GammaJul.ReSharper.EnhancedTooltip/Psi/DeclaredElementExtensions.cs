using GammaJul.ReSharper.EnhancedTooltip.Presentation;
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

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

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

		[NotNull]
		public static string GetElementKindString([CanBeNull] this IDeclaredElement element) {
			if (element == null)
				return "unknown";

			if (element is INamespace)
				return "namespace";
			if (element is IClass)
				return "class";
			if (element is IInterface)
				return "interface";
			if (element is IStruct)
				return "struct";
			if (element is IDelegate)
				return "delegate";
			if (element is IEnum)
				return "enum";
			if (element is ILabel)
				return "label";
			if (element is ITypeParameter)
				return "type parameter";
			if (element is IAnonymousMethod)
				return "anonymous method";
			if (element is IAlias)
				return "alias";
			if (element is IQueryRangeVariable || element is IQueryVariable || element is IQueryAnonymousTypeProperty)
				return "range variable";
			if (element is IAnonymousTypeProperty)
				return "property";
			if (element is IEvent)
				return "event";
			if (element is IParameter)
				return "parameter";

			if (element is IFunction) {
				if (element is IAccessor)
					return "accessor";
				if (element is IMethod)
					return CSharpDeclaredElementUtil.IsDestructor(element) ? "destructor" : "method";
				if (element is IConstructor)
					return "constructor";
				if (element is IOperator)
					return "operator";
			}
			
			var field = element as IField;
			if (field != null) {
				if (field.IsField)
					return "field";
				if (field.IsConstant)
					return "constant";
				if (field.IsEnumMember)
					return "enum member";
			}

			var property = element as IProperty;
			if (property != null) {
				if (CSharpDeclaredElementUtil.IsIndexer(property))
					return "indexer";
				return CSharpDeclaredElementUtil.IsIndexedProperty(property) ? "indexed property" : "property";
			}
			
			var localVariable = element as ILocalVariable;
			if (localVariable != null)
				return localVariable.IsConstant ? "local constant" : "local variable";
			
			return "unknown";
		}

	}

}