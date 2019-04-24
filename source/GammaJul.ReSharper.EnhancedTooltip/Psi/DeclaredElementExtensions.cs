using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree.Query;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	internal static class DeclaredElementExtensions {

		[NotNull]
		public static string GetElementKindString(
			[CanBeNull] this IDeclaredElement element,
			bool useExtensionMethodKind,
			bool useAttributeTypeKind,
			bool useClassModifiers,
			bool useStructModifiers,
			bool useMethodModifiers) {

			if (element == null)
				return "unknown";

			if (element is IClass @class) {
				string classModifiers = useClassModifiers ? GetClassModifiersDisplay(@class) : null;
				string classKind = useAttributeTypeKind && @class.IsAttribute() ? "attribute" : "class";
				return classModifiers + classKind;
			}

			if (element is IStruct @struct) {
				string structModifiers = useStructModifiers ? GetStructModifiersDisplay(@struct) : null;
				return structModifiers + "struct";
			}

			if (element is INamespace)
				return "namespace";
			if (element is IInterface)
				return "interface";
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
			if (element is ILocalFunction)
				return "local function";

			if (element is IFunction) {
				if (element is IAccessor)
					return "accessor";

				if (element is IMethod method) {
					if (useExtensionMethodKind && method.IsExtensionMethod)
						return "extension";
					if (CSharpDeclaredElementUtil.IsDestructor(method))
						return "destructor";
					return (useMethodModifiers ? GetMethodModifiersDisplay(method) : null) + "method";
				}

				if (element is IConstructor)
					return "constructor";
				if (element is IOperator)
					return "operator";
			}

			if (element is IField field) {
				if (field.IsField)
					return "field";
				if (field.IsConstant)
					return "constant";
				if (field.IsEnumMember)
					return "enum member";
			}

			if (element is IProperty property) {
				if (CSharpDeclaredElementUtil.IsIndexer(property))
					return "indexer";
				if (property.IsCSharpIndexedProperty())
					return "indexed property";
				return "property";
			}

			if (element is ILocalVariable localVariable)
				return localVariable.IsConstant ? "local constant" : "local variable";
			
			return "unknown";
		}

		[CanBeNull]
		private static string GetClassModifiersDisplay([NotNull] IModifiersOwner modifiersOwner) {
			string display = null;

			if (modifiersOwner.IsAbstract)
				display = modifiersOwner.IsSealed ? "static " : "abstract ";
			else if (modifiersOwner.IsSealed)
				display = "sealed ";

			if (modifiersOwner.IsUnsafe)
				display += "unsafe ";

			return display;
		}

		[CanBeNull]
		private static string GetStructModifiersDisplay([NotNull] IStruct @struct) {
			string display = null;

			if (@struct.IsByRefLike)
				display += "ref ";

			if (@struct.IsReadonly)
				display += "readonly ";

			return display;
		}

		[CanBeNull]
		private static string GetMethodModifiersDisplay([NotNull] IModifiersOwner modifiersOwner) {
			string display = null;

			if (modifiersOwner.IsStatic)
				display = "static ";
			else {
				if (modifiersOwner.IsSealed)
					display = "sealed ";
				else if (modifiersOwner.IsAbstract)
					display = "abstract ";
				if (modifiersOwner.IsOverride)
					display += "override ";
				else if (modifiersOwner.IsVirtual && !modifiersOwner.IsAbstract)
					display += "virtual ";
			}
			
			if (modifiersOwner.IsUnsafe)
				display += "unsafe ";
			if (modifiersOwner.IsExtern)
				display += "extern ";

			return display;
		}
		
		[Pure]
		[NotNull]
		public static IDeclaredElement EliminateDelegateInvokeMethod([NotNull] this IDeclaredElement declaredElement)
			=> DeclaredElementUtil.EliminateDelegateInvokeMethod(declaredElement);

	}

}