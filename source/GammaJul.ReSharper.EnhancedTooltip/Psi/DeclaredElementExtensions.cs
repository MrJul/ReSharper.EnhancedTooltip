using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree.Query;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Psi {

	internal static class DeclaredElementExtensions {

		public static string GetElementKindString(
			this IDeclaredElement? element,
			bool useExtensionMethodKind,
			bool useAttributeTypeKind,
			bool useClassModifiers,
			bool useStructModifiers,
			bool useMethodModifiers) {

			return element switch {
				null => "unknown",
				IRecord record => element is IStruct @struct ? GetStructModifiersDisplay(@struct, useStructModifiers) + "record struct" : GetClassModifiersDisplay(record, useClassModifiers) + (useAttributeTypeKind && record.IsAttribute() ? "attribute " : "record"),
        IClass @class => GetClassModifiersDisplay(@class, useClassModifiers) + (useAttributeTypeKind && @class.IsAttribute() ? "attribute" : "class"),
				IStruct @struct => GetStructModifiersDisplay(@struct, useStructModifiers) + (element is IRecord ? "record struct" : "struct"),
				INamespace => "namespace",
				IInterface => "interface",
				IDelegate => "delegate",
				IEnum => "enum",
				ILabel => "label",
				ITypeParameter => "type parameter",
				IAnonymousMethod => "anonymous method",
				IAlias => "alias",
				IQueryRangeVariable or IQueryVariable or IQueryAnonymousTypeProperty => "range variable",
				IAnonymousTypeProperty => "property",
				IEvent => "event",
				IParameter => "parameter",
				ILocalFunction => "local function",
				IAccessor => "accessor",
				IMethod { IsExtensionMethod: true } when useExtensionMethodKind => "extension",
				IMethod method when method.IsDestructor() => "destructor",
				IMethod method => GetMethodModifiersDisplay(method, useMethodModifiers) + "method",
				IConstructor => "constructor",
				IOperator => "operator",
				IField { IsField: true } => "field",
				IField { IsConstant: true } => "constant",
				IField { IsEnumMember: true } => "enum member",
				IProperty property when CSharpDeclaredElementUtil.IsIndexer(property) => "indexer",
				IProperty property when property.IsCSharpIndexedProperty() => "indexed property",
				IProperty => "property",
				ILocalVariable { IsConstant: true } => "local constant",
				ILocalVariable => "local variable",
				_ => "unknown"
			};

		}

		private static string? GetClassModifiersDisplay(IModifiersOwner modifiersOwner, bool useModifiers) {
			if (!useModifiers)
				return null;
			
			string? display = null;

			if (modifiersOwner.IsAbstract)
				display = modifiersOwner.IsSealed ? "static " : "abstract ";
			else if (modifiersOwner.IsSealed)
				display = "sealed ";

			if (modifiersOwner.IsUnsafe)
				display += "unsafe ";

			return display;
		}

		private static string? GetStructModifiersDisplay(IStruct @struct, bool useModifiers) {
			if (!useModifiers)
				return null;
			
			string? display = null;

			if (@struct.IsByRefLike)
				display += "ref ";

			if (@struct.IsReadonly)
				display += "readonly ";

			return display;
		}

		private static string? GetMethodModifiersDisplay(IModifiersOwner modifiersOwner, bool useModifiers) {
			if (!useModifiers)
				return null;
			
			string? display = null;

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
		public static IDeclaredElement EliminateDelegateInvokeMethod(this IDeclaredElement declaredElement)
			=> DeclaredElementUtil.EliminateDelegateInvokeMethod(declaredElement);

	}

}