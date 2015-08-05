using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree.Query;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	internal static class DeclaredElementExtensions {
		
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
				
				var method = element as IMethod;
				if (method != null) {
					if (method.IsExtensionMethod)
						return "extension";
					if (CSharpDeclaredElementUtil.IsDestructor(method))
						return "destructor";
					return "method";
				}

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
				if (property.IsIndexedProperty())
					return "indexed property";
				return "property";
			}

			var localVariable = element as ILocalVariable;
			if (localVariable != null)
				return localVariable.IsConstant ? "local constant" : "local variable";
			
			return "unknown";
		}

		[Pure]
		[NotNull]
		public static IDeclaredElement EliminateDelegateInvokeMethod([NotNull] this IDeclaredElement declaredElement)
			=> DeclaredElementUtil.EliminateDelegateInvokeMethod(declaredElement);

	}

}