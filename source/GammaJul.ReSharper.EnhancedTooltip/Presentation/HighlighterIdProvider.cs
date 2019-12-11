using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.CSharp.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
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
			=> Get(CSharpHighlightingAttributeIds.ACCESSOR, VsKeyword, VsKeyword, VsKeyword);

		[NotNull]
		public string Class
			=> Get(CSharpHighlightingAttributeIds.CLASS, "class name", "class name", "User Types");

		[NotNull]
		public string Constant
			=> Get(CSharpHighlightingAttributeIds.CONSTANT, "constant name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Delegate
			=> Get(CSharpHighlightingAttributeIds.DELEGATE, "delegate name", "delegate name", "User Types(Delegates)");

		[NotNull]
		public string Enum
			=> Get(CSharpHighlightingAttributeIds.ENUM, "enum name", "enum name", "User Types(Enums)");

		[NotNull]
		public string ExtensionMethod
			=> Get(CSharpHighlightingAttributeIds.EXTENSION_METHOD, "extension method name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Event
			=> Get(CSharpHighlightingAttributeIds.EVENT, "event name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Field
			=> Get(CSharpHighlightingAttributeIds.FIELD, "field name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Keyword
			=> VsKeyword;

		[NotNull]
		public string Identifier
			=> VsIdentifier;

		[NotNull]
		public string Interface
			=> Get(CSharpHighlightingAttributeIds.INTERFACE, "interface name", "interface name", "User Types(Interfaces)");

		[NotNull]
		public string LocalFunction
			=> Get(CSharpHighlightingAttributeIds.LOCAL_FUNCTION, "method name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string LocalVariable
			=> Get(CSharpHighlightingAttributeIds.LOCAL_VARIABLE, "local name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Method
			=> Get(CSharpHighlightingAttributeIds.METHOD, "method name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Namespace
			=> Get(CSharpHighlightingAttributeIds.NAMESPACE, "namespace name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Number
			=> "Number";

		[NotNull]
		public string Parameter
			=> Get(CSharpHighlightingAttributeIds.PARAMETER, "parameter name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string Path
			=> Get(GeneralHighlightingAttributeIds.PATH, VsIdentifier, VsIdentifier, VsIdentifier);

		[NotNull]
		public string Operator
			=> VsOperator;

		[NotNull]
		public string Property
			=> Get(CSharpHighlightingAttributeIds.PROPERTY, "property name", VsIdentifier, VsIdentifier);

		[NotNull]
		public string StaticClass
			=> Get(CSharpHighlightingAttributeIds.STATIC_CLASS, "class name", "class name", "User Types");

		[NotNull]
		public string String
			=> "String";

		[NotNull]
		public string Struct
			=> Get(CSharpHighlightingAttributeIds.STRUCT, "struct name", "struct name", "User Types(Value types)");

		[CanBeNull]
		public string TypeParameter
			// Note: there is an "User Types(Type parameters)" on old VS versions but it's actually never used.
			=> Get(CSharpHighlightingAttributeIds.TYPE_PARAMETER, "type parameter name", "type parameter name", null);

		[NotNull]
		public string UserOperator
			=> Get(CSharpHighlightingAttributeIds.OVERLOADED_OPERATOR, VsOperator, VsOperator, VsOperator);

		[CanBeNull]
		public string TupleComponentName
			=> Get(CSharpHighlightingAttributeIds.TUPLE_COMPONENT_NAME, VsIdentifier, VsIdentifier, null);

		[CanBeNull]
		public string GetForTypeElement([CanBeNull] ITypeElement typeElement) {
			if (typeElement == null)
				return null;

			if (_highlighterIdSource == HighlighterIdSource.ReSharper) {
				var language = CSharpLanguage.Instance;
				if (language != null) {
					return typeElement
						.GetPsiServices()
						.LanguageManager
						.TryGetService<IHighlightingAttributeIdProvider>(language)
						?.GetHighlightingAttributeId(typeElement, false);
				}
				return null;
			}

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