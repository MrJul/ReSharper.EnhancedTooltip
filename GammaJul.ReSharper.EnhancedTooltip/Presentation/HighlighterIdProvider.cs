using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree.Query;
using JetBrains.ReSharper.Psi.CSharp.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// Returns the name of a classification, depending on whether ReSharper's colors are enabled, and the current VS version.
	/// </summary>
	public class HighlighterIdProvider {

		private readonly bool _useReSharperColors;
		private readonly bool _useRoslynColors;

		[ContractAnnotation("roslynVsColor: null => canbenull")]
		[ContractAnnotation("legacyVsColor: null => canbenull")]
		private string Get([NotNull] string resharperColor, [CanBeNull] string roslynVsColor, [CanBeNull] string legacyVsColor) {
			if (_useReSharperColors)
				return resharperColor;
			if (_useRoslynColors)
				return roslynVsColor;
			return legacyVsColor;
		}

		private const string VsIdentifier = "Identifier";
		private const string VsOperator = "Operator";

		[NotNull]
		public string Class
			=> Get(HighlightingAttributeIds.TYPE_CLASS_ATTRIBUTE, "class name", "User Types");

		[NotNull]
		public string Constant
			=> Get(HighlightingAttributeIds.CONSTANT_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);
		
		[NotNull]
		public string Delegate
			=> Get(HighlightingAttributeIds.TYPE_DELEGATE_ATTRIBUTE, "delegate name", "User Types(Delegates)");

		[NotNull]
		public string Enum
			=> Get(HighlightingAttributeIds.TYPE_ENUM_ATTRIBUTE, "enum name", "User Types(Enums)");

		[NotNull]
		public string ExtensionMethod
			=> Get(HighlightingAttributeIds.EXTENSION_METHOD_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Event
			=> Get(HighlightingAttributeIds.EVENT_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Field
			=> Get(HighlightingAttributeIds.FIELD_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Keyword
			=> "Keyword";

		[NotNull]
		public string Identifier
			=> VsIdentifier;

		[NotNull]
		public string Interface
			=> Get(HighlightingAttributeIds.TYPE_INTERFACE_ATTRIBUTE, "interface name", "User Types(Interfaces)");

		[NotNull]
		public string LocalVariable
			=> Get(HighlightingAttributeIds.LOCAL_VARIABLE_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Method
			=> Get(HighlightingAttributeIds.METHOD_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Namespace
			=> Get(HighlightingAttributeIds.NAMESPACE_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Number
			=> "Number";

		[NotNull]
		public string Parameter
			=> Get(HighlightingAttributeIds.PARAMETER_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Path
			=> Get(HighlightingAttributeIds.PATH_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Operator
			=> VsOperator;

		[NotNull]
		public string Property
			=> Get(HighlightingAttributeIds.FIELD_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier);

		[NotNull]
		public string StaticClass
			=> Get(HighlightingAttributeIds.TYPE_STATIC_CLASS_ATTRIBUTE, "class name", "User Types");

		[NotNull]
		public string String
			=> "String";

		[NotNull]
		public string Struct
			=> Get(HighlightingAttributeIds.TYPE_STRUCT_ATTRIBUTE, "struct name", "User Types(Value types)");

		[CanBeNull]
		public string TypeParameter
			// Note: there is an "User Types(Type parameters)" on old VS versions but it's actually never.
			=> Get(HighlightingAttributeIds.TYPE_PARAMETER_ATTRIBUTE, "type parameter name", null);
		
		[NotNull]
		public string UserOperator
			=> Get(HighlightingAttributeIds.OPERATOR_IDENTIFIER_ATTRIBUTE, VsOperator, VsOperator);

		[CanBeNull]
		public string GetForTypeElement([CanBeNull] ITypeElement typeElement) {
			if (typeElement == null)
				return null;

			if (_useReSharperColors)
				return HighlightingAttributeIds.GetHighlightAttributeForTypeElement(typeElement);

			if (typeElement is IDelegate)
				return Delegate;
			if (typeElement is IEnum)
				return Enum;
			if (typeElement is IInterface)
				return Interface;
			if (typeElement is IStruct)
				return Struct;
			if (typeElement is ITypeParameter)
				return TypeParameter;
			var @class = typeElement as IClass;
			if (@class != null)
				return @class.IsAbstract && @class.IsSealed ? StaticClass : Class;

			return VsIdentifier;
		}

		[CanBeNull]
		public string GetForDeclaredElement([CanBeNull] IDeclaredElement declaredElement) {
			if (declaredElement == null)
				return null;

			if (declaredElement is IFunction) {
				
				var method = declaredElement as IMethod;
				if (method != null) {
					if (method.IsPredefined)
						return null;
					return method.IsExtensionMethod ? ExtensionMethod : Method;
				}

				var signOperator = declaredElement as ISignOperator;
				if (signOperator != null)
					return signOperator.IsPredefined ? Operator : UserOperator;

				var constructor = declaredElement as IConstructor;
				if (constructor != null)
					return GetForTypeElement(constructor.GetContainingType());

			}

			var field = declaredElement as IField;
			if (field != null)
				return field.IsField ? Field : Constant;

			var typeElement = declaredElement as ITypeElement;
			if (typeElement != null)
				return GetForTypeElement(typeElement);
			
			if (CSharpDeclaredElementUtil.IsProperty(declaredElement) || declaredElement.IsCSharpIndexedProperty())
				return Property;

			if (declaredElement is IEvent)
				return Event;

			if (declaredElement is INamespace)
				return Namespace;

			var parameter = declaredElement as IParameter;
			if (parameter != null)
				return parameter.IsValueVariable ? Identifier : Parameter;

			var localVariable = declaredElement as ILocalVariable;
			if (localVariable != null)
				return localVariable.IsConstant ? Constant : LocalVariable;

			if (declaredElement is ICSharpAnonymousTypeProperty)
				return declaredElement is IQueryAnonymousTypeProperty ? LocalVariable : Field;

			if (declaredElement is IPathDeclaredElement)
				return Path;

			return Identifier;
		}

		public HighlighterIdProvider(bool useReSharperColors, bool useRoslynColors) {
			_useReSharperColors = useReSharperColors;
			_useRoslynColors = useRoslynColors;
		}

	}

}