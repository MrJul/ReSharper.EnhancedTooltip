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

		private string Get(string rsColor, string vs16Color, string vs14Color, string vsLegacyColor)
			=> _highlighterIdSource switch {
				HighlighterIdSource.ReSharper => rsColor,
				HighlighterIdSource.VisualStudio16 => vs16Color,
				HighlighterIdSource.VisualStudio14 => vs14Color,
				_ => vsLegacyColor
			};
		
		private string? Get(string rsColor, string vs16Color, string vs14Color)
			=> _highlighterIdSource switch {
				HighlighterIdSource.ReSharper => rsColor,
				HighlighterIdSource.VisualStudio16 => vs16Color,
				HighlighterIdSource.VisualStudio14 => vs14Color,
				_ => null
			};

		private const string VsIdentifier = "Identifier";
		private const string VsOperator = "Operator";
		private const string VsKeyword = "Keyword";

		public string Accessor
			=> Get(CSharpHighlightingAttributeIds.ACCESSOR, VsKeyword, VsKeyword, VsKeyword);

		public string Class
			=> Get(CSharpHighlightingAttributeIds.CLASS, "class name", "class name", "User Types");

		public string Constant
			=> Get(CSharpHighlightingAttributeIds.CONSTANT, "constant name", VsIdentifier, VsIdentifier);

		public string Delegate
			=> Get(CSharpHighlightingAttributeIds.DELEGATE, "delegate name", "delegate name", "User Types(Delegates)");

		public string Enum
			=> Get(CSharpHighlightingAttributeIds.ENUM, "enum name", "enum name", "User Types(Enums)");

		public string ExtensionMethod
			=> Get(CSharpHighlightingAttributeIds.EXTENSION_METHOD, "extension method name", VsIdentifier, VsIdentifier);

		public string Event
			=> Get(CSharpHighlightingAttributeIds.EVENT, "event name", VsIdentifier, VsIdentifier);

		public string Field
			=> Get(CSharpHighlightingAttributeIds.FIELD, "field name", VsIdentifier, VsIdentifier);

		public string Keyword
			=> VsKeyword;

		public string Identifier
			=> VsIdentifier;

		public string Interface
			=> Get(CSharpHighlightingAttributeIds.INTERFACE, "interface name", "interface name", "User Types(Interfaces)");

		public string LocalFunction
			=> Get(CSharpHighlightingAttributeIds.LOCAL_FUNCTION, "method name", VsIdentifier, VsIdentifier);

		public string LocalVariable
			=> Get(CSharpHighlightingAttributeIds.LOCAL_VARIABLE, "local name", VsIdentifier, VsIdentifier);

		public string Method
			=> Get(CSharpHighlightingAttributeIds.METHOD, "method name", VsIdentifier, VsIdentifier);

		public string Namespace
			=> Get(CSharpHighlightingAttributeIds.NAMESPACE, "namespace name", VsIdentifier, VsIdentifier);

		public string Number
			=> "Number";

		public string Parameter
			=> Get(CSharpHighlightingAttributeIds.PARAMETER, "parameter name", VsIdentifier, VsIdentifier);

		public string Path
			=> Get(GeneralHighlightingAttributeIds.PATH, VsIdentifier, VsIdentifier, VsIdentifier);

		public string Operator
			=> VsOperator;

		public string Property
			=> Get(CSharpHighlightingAttributeIds.PROPERTY, "property name", VsIdentifier, VsIdentifier);

		public string StaticClass
			=> Get(CSharpHighlightingAttributeIds.STATIC_CLASS, "class name", "class name", "User Types");

		public string String
			=> "String";

		public string Struct
			=> Get(CSharpHighlightingAttributeIds.STRUCT, "struct name", "struct name", "User Types(Value types)");

		public string? TypeParameter
			// Note: there is an "User Types(Type parameters)" on old VS versions but it's actually never used.
			=> Get(CSharpHighlightingAttributeIds.TYPE_PARAMETER, "type parameter name", "type parameter name");

		public string UserOperator
			=> Get(CSharpHighlightingAttributeIds.OVERLOADED_OPERATOR, VsOperator, VsOperator, VsOperator);

		public string? TupleComponentName
			=> Get(CSharpHighlightingAttributeIds.TUPLE_COMPONENT_NAME, VsIdentifier, VsIdentifier);

		public string? GetForTypeElement(ITypeElement? typeElement) {
			if (typeElement is null)
				return null;

			if (_highlighterIdSource == HighlighterIdSource.ReSharper) {
				if (CSharpLanguage.Instance is { } language) {
					return typeElement
						.GetPsiServices()
						.LanguageManager
						.TryGetService<IHighlightingAttributeIdProvider>(language)
						?.GetHighlightingAttributeId(typeElement, false);
				}
				return null;
			}

			return typeElement switch {
				IDelegate => Delegate,
				IEnum => Enum,
				IInterface => Interface,
				IStruct => Struct,
				ITypeParameter => TypeParameter,
				IClass @class => @class.IsAbstract && @class.IsSealed ? StaticClass : Class,
				_ => VsIdentifier
			};
		}

		public string? GetForDeclaredElement(IDeclaredElement? declaredElement)
			=> declaredElement switch {
				null => null,
				IMethod { IsPredefined: true } => null,
				IMethod { IsExtensionMethod: true } => ExtensionMethod,
				IMethod => Method,
				ISignOperator { IsPredefined: true } => Operator,
				ISignOperator => UserOperator,
				IConstructor constructor => GetForTypeElement(constructor.GetContainingType()),
				IField { IsField: false } => Constant,
				IField => Field,
				ITypeElement typeElement => GetForTypeElement(typeElement),
				IEvent => Event,
				INamespace => Namespace,
				IParameter { IsValueVariable: true } => Identifier,
				IParameter => Parameter,
				ILocalVariable { IsConstant: true } => Constant,
				ILocalVariable => LocalVariable,
				IQueryAnonymousTypeProperty => LocalVariable,
				ICSharpAnonymousTypeProperty => Field,
				ILocalFunction => LocalFunction,
				IPathDeclaredElement => Path,
				_ when CSharpDeclaredElementUtil.IsProperty(declaredElement) || declaredElement.IsCSharpIndexedProperty() => Property,
				_ => Identifier
			};

		public HighlighterIdProvider(HighlighterIdSource highlighterIdSource)
			=> _highlighterIdSource = highlighterIdSource;

	}

}