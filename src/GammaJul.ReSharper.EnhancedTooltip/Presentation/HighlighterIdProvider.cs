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

		private readonly HighlighterIdSource _highlighterIdSource;

		[ContractAnnotation("vsLegacyColor: null => canbenull")]
		private string Get([NotNull] string rsColor, [NotNull] string vs16Color, [NotNull] string vs14Color, [CanBeNull] string vsLegacyColor) {
			switch (_highlighterIdSource) {
				case HighlighterIdSource.ReSharper:
					return rsColor;
				case HighlighterIdSource.VisualStudio16:
					return vs16Color;
				case HighlighterIdSource.VisualStudio14:
					return vs14Color;
				default:
					return vsLegacyColor;
			}
		}

		private const string VsIdentifier = "Identifier";
		private const string VsOperator = "Operator";
		private const string VsKeyword = "Keyword";

		[NotNull]
		public string Accessor
			=> Get(HighlightingAttributeIds.METHOD_IDENTIFIER_ATTRIBUTE, VsKeyword, VsKeyword, VsKeyword);

		[NotNull]
		public string Class
			=> Get(HighlightingAttributeIds.TYPE_CLASS_ATTRIBUTE, "class name", "class name", "User Types");

		[NotNull]
		public string Constant
			=> Get(HighlightingAttributeIds.CONSTANT_IDENTIFIER_ATTRIBUTE, "constant name", VsIdentifier, VsIdentifier);
		
		[NotNull]
		public string Delegate
			=> Get(HighlightingAttributeIds.TYPE_DELEGATE_ATTRIBUTE, "delegate name", "delegate name", "User Types(Delegates)");

		[NotNull]
		public string Enum
			=> Get(HighlightingAttributeIds.TYPE_ENUM_ATTRIBUTE, "enum name", "enum name", "User Types(Enums)");

		[NotNull]
		public string ExtensionMethod
			=> Get(HighlightingAttributeIds.EXTENSION_METHOD_IDENTIFIER_ATTRIBUTE, "extension method name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Event
			=> Get(HighlightingAttributeIds.EVENT_IDENTIFIER_ATTRIBUTE, "event name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Field
			=> Get(HighlightingAttributeIds.FIELD_IDENTIFIER_ATTRIBUTE, "field name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Keyword
			=> VsKeyword;

		[NotNull]
		public string Identifier
			=> VsIdentifier;

		[NotNull]
		public string Interface
			=> Get(HighlightingAttributeIds.TYPE_INTERFACE_ATTRIBUTE, "interface name", "interface name", "User Types(Interfaces)");

		[NotNull]
		public string LocalFunction
			=> Get(HighlightingAttributeIds.LOCAL_FUNCTION_IDENTIFIER_ATTRIBUTE, "method name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string LocalVariable
			=> Get(HighlightingAttributeIds.LOCAL_VARIABLE_IDENTIFIER_ATTRIBUTE, "local name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Method
			=> Get(HighlightingAttributeIds.METHOD_IDENTIFIER_ATTRIBUTE, "method name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Namespace
			=> Get(HighlightingAttributeIds.NAMESPACE_IDENTIFIER_ATTRIBUTE, "namespace name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Number
			=> "Number";

		[NotNull]
		public string Parameter
			=> Get(HighlightingAttributeIds.PARAMETER_IDENTIFIER_ATTRIBUTE, "parameter name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Path
			=> Get(HighlightingAttributeIds.PATH_IDENTIFIER_ATTRIBUTE, VsIdentifier, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Operator
			=> VsOperator;

		[NotNull]
		public string Property
			=> Get(HighlightingAttributeIds.PROPERTY_IDENTIFIER_ATTRIBUTE, "property name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string StaticClass
			=> Get(HighlightingAttributeIds.TYPE_STATIC_CLASS_ATTRIBUTE, "class name", "class name", "User Types");

		[NotNull]
		public string String
			=> "String";

		[NotNull]
		public string Struct
			=> Get(HighlightingAttributeIds.TYPE_STRUCT_ATTRIBUTE, "struct name", "struct name", "User Types(Value types)");

		[CanBeNull]
		public string TypeParameter
			// Note: there is an "User Types(Type parameters)" on old VS versions but it's actually never used.
			=> Get(HighlightingAttributeIds.TYPE_PARAMETER_ATTRIBUTE, "type parameter name", "type parameter name", null);
		
		[NotNull]
		public string UserOperator
			=> Get(HighlightingAttributeIds.OPERATOR_IDENTIFIER_ATTRIBUTE, VsOperator, VsOperator, VsOperator);

		[CanBeNull]
		public string TupleComponentName
			=> Get(HighlightingAttributeIds.TUPLE_COMPONENT_NAME_ATTRIBUTE, VsIdentifier, VsIdentifier, null);

		[CanBeNull]
		public string GetForTypeElement([CanBeNull] ITypeElement typeElement) {
			if (typeElement == null)
				return null;

			if (_highlighterIdSource == HighlighterIdSource.ReSharper)
				return HighlightingAttributeIds.GetHighlightAttributeForTypeElement(typeElement);

			switch (typeElement) {
				case IDelegate _:
					return Delegate;
				case IEnum _:
					return Enum;
				case IInterface _:
					return Interface;
				case IStruct _:
					return Struct;
				case ITypeParameter _:
					return TypeParameter;
				case IClass @class:
					return @class.IsAbstract && @class.IsSealed ? StaticClass : Class;
				default:
					return VsIdentifier;
			}
		}

		[CanBeNull]
		public string GetForDeclaredElement([CanBeNull] IDeclaredElement declaredElement) {
			switch (declaredElement) {

				case null:
					return null;

				case IMethod method:
					if (method.IsPredefined)
						return null;
					return method.IsExtensionMethod ? ExtensionMethod : Method;

				case ISignOperator signOperator:
					return signOperator.IsPredefined ? Operator : UserOperator;

				case IConstructor constructor:
					return GetForTypeElement(constructor.GetContainingType());

				case IField field:
					return field.IsField ? Field : Constant;

				case ITypeElement typeElement:
					return GetForTypeElement(typeElement);
					
				case IEvent _:
					return Event;

				case INamespace _:
					return Namespace;

				case IParameter parameter:
					return parameter.IsValueVariable ? Identifier : Parameter;

				case ILocalVariable localVariable:
					return localVariable.IsConstant ? Constant : LocalVariable;

				case ICSharpAnonymousTypeProperty _:
					return declaredElement is IQueryAnonymousTypeProperty ? LocalVariable : Field;

				case ILocalFunction _:
					return LocalFunction;

				case IPathDeclaredElement _:
					return Path;

				case var _ when CSharpDeclaredElementUtil.IsProperty(declaredElement) || declaredElement.IsCSharpIndexedProperty():
					return Property;

				default:
					return Identifier;

			}
		}

		public HighlighterIdProvider(HighlighterIdSource highlighterIdSource) {
			_highlighterIdSource = highlighterIdSource;
		}

	}

}