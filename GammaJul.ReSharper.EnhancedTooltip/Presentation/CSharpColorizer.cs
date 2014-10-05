using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree.Query;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// Implementation of <see cref="IColorizer"/> for C#.
	/// </summary>
	/// <remarks>
	/// This class mimics a lot of functionality from ReSharper's <see cref="CSharpDeclaredElementPresenter"/>.
	/// Originally, I wanted to reuse the presenter and use the returned <see cref="DeclaredElementPresenterMarking"/> to colorize the needed parts.
	/// However, there isn't enough information in the marking to do this correctly: rewriting it completely provides more flexibility.
	/// Plus, we can provide additional options.
	/// </remarks>
	internal sealed class CSharpColorizer : IColorizer {

		private static readonly DeclaredElementPresenterStyle _specialTypeStyle = new DeclaredElementPresenterStyle(NameStyle.SHORT) {
			ShowTypeParameters = TypeParameterStyle.NONE
		};

		private static readonly string _notNullAttributeName = CodeAnnotationsCache.NotNullAttributeShortName.TrimFromEnd("Attribute", StringComparison.Ordinal);
		private static readonly string _canBeNullAttributeName = CodeAnnotationsCache.CanBeNullAttributeShortName.TrimFromEnd("Attribute", StringComparison.Ordinal);

		private readonly RichText _richText;
		private readonly PresenterOptions _options;
		private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		private readonly PresentedInfo _presentedInfo;
		private readonly CodeAnnotationsCache _codeAnnotationsCache;

		public void AppendDeclaredElement(IDeclaredElement element, ISubstitution substitution, string nameHighlightingAttributeId) {
			if (!IsClrPresentableElement(element))
				return;

			if (_options.ShowElementKind)
				AppendElementKind(element);

			if (_options.ShowAccessRights)
				AppendAccessRights(element);
			if (_options.ShowModifiers)
				AppendModifiers(element);

			if (_options.ShowElementNullness) {
				var attributesOwner = element as IAttributesOwner;
				if (attributesOwner != null)
					AppendNullness(attributesOwner);
			}

			if (_options.ShowElementType == ElementTypeDisplay.Before)
				AppendElementType(element, substitution, NamespaceDisplays.ElementType, null, " ");

			if (_options.ShowName)
				AppendNameWithContainer(element, substitution, nameHighlightingAttributeId);
			if (_options.ShowParametersType || _options.ShowParametersName)
				AppendParameters(element, substitution, true);

			if (_options.ShowElementType == ElementTypeDisplay.After)
				AppendElementType(element, substitution, NamespaceDisplays.ElementType, ":", null);
		}

		private static bool IsClrPresentableElement([NotNull] IDeclaredElement element) {
			return element.IsValid()
				&& element is IClrDeclaredElement
				&& element.GetElementType().IsPresentable(CSharpLanguage.Instance);
		}

		private void AppendText([CanBeNull] string text, [CanBeNull] string highlighterAttributeId) {
			if (text.IsEmpty())
				return;

			TextStyle textStyle = _textStyleHighlighterManager.GetHighlighterTextStyle(highlighterAttributeId);
			_richText.Append(text, textStyle);
		}

		private void AppendText([CanBeNull] string text, TextStyle textStyle) {
			if (!text.IsEmpty())
				_richText.Append(text, textStyle);
		}

		private void AppendElementKind([NotNull] IDeclaredElement element) {
			AppendText("(" + GetElementKind(element) + ") ", new TextStyle(FontStyle.Italic));
		}

		[NotNull]
		private static string GetElementKind(IDeclaredElement element) {
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

		private void AppendAccessRights([NotNull] IDeclaredElement element) {
			var accessRightsOwner = element as IAccessRightsOwner;
			if (accessRightsOwner == null)
				return;

			string accessRights = CSharpDeclaredElementPresenter.Instance.Format(accessRightsOwner.GetAccessRights());
			if (!accessRights.IsEmpty())
				AppendText(accessRights + " ", VsHighlightingAttributeIds.Keyword);
		}

		private void AppendModifiers([NotNull] IDeclaredElement element) {
			var modifiersOwner = element as IModifiersOwner;
			if (modifiersOwner == null || modifiersOwner is IAccessor || CSharpDeclaredElementUtil.IsDestructor(element))
				return;

			var builder = new StringBuilder();
			if (element is ITypeElement) {
				if (modifiersOwner.IsAbstract && modifiersOwner.IsSealed)
					builder.Append("static ");
				else {
					if (modifiersOwner.IsSealed && !(element is IStruct))
						builder.Append("sealed ");
					if (modifiersOwner.IsAbstract && !(element is IInterface))
						builder.Append("abstract ");
				}
			}
			else {
				if (modifiersOwner.IsAbstract)
					builder.Append("abstract ");
				if (modifiersOwner.IsSealed)
					builder.Append("sealed ");
				if (modifiersOwner.IsStatic)
					builder.Append("static ");
			}
			if (modifiersOwner.IsVirtual)
				builder.Append("virtual ");
			if (modifiersOwner.IsOverride)
				builder.Append("override ");
			if (modifiersOwner.IsExtern)
				builder.Append("extern ");
			if (modifiersOwner.IsReadonly)
				builder.Append("readonly ");
			if (modifiersOwner.IsUnsafe)
				builder.Append("unsafe ");
			if (modifiersOwner.IsVolatile)
				builder.Append("volatile ");

			if (builder.Length > 0)
				AppendText(builder.ToString(), VsHighlightingAttributeIds.Keyword);
		}

		private void AppendElementType([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, NamespaceDisplays namespaceDisplays,
			[CanBeNull] string before, [CanBeNull] string after) {
			// Use the special type first if available (eg Razor helper), colorize it as a keyword
			string specialTypeString = CSharpModificationUtil.GetSpecialElementType(_specialTypeStyle, element, substitution);
			if (!specialTypeString.IsEmpty()) {
				AppendText(before, null);
				AppendText(specialTypeString, VsHighlightingAttributeIds.Keyword);
				AppendText(after, null);
				return;
			}

			IType elementType = GetElementType(element, substitution);
			if (elementType == null)
				return;

			AppendText(before, null);
			AppendType(elementType, namespaceDisplays);
			AppendText(after, null);
		}

		[CanBeNull]
		private static IType GetElementType([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution) {
			if (element is IConstructor || CSharpDeclaredElementUtil.IsDestructor(element))
				return null;
			
			var typeOwner = element as ITypeOwner;
			if (typeOwner != null)
				return substitution.Apply(typeOwner.Type);

			var parametersOwner = element as IParametersOwner;
			if (parametersOwner != null)
				return substitution.Apply(parametersOwner.ReturnType);

			var deleg = element as IDelegate;
			if (deleg != null)
				return substitution.Apply(deleg.InvokeMethod.ReturnType);

			return null;
		}

		private void AppendType([NotNull] IType type, NamespaceDisplays expectedNamespaceDisplay) {
			var arrayType = type as IArrayType;
			if (arrayType != null) {
				AppendArrayType(arrayType, expectedNamespaceDisplay);
				return;
			}

			var pointerType = type as IPointerType;
			if (pointerType != null) {
				AppendPointerType(pointerType, expectedNamespaceDisplay);
				return;
			}

			var declaredType = type as IDeclaredType;
			if (declaredType != null)
				AppendDeclaredType(declaredType, expectedNamespaceDisplay);
		}

		private void AppendArrayType([NotNull] IArrayType arrayType, NamespaceDisplays expectedNamespaceDisplay) {
			AppendType(arrayType.ElementType, expectedNamespaceDisplay);
			AppendText("[" + new string(',', arrayType.Rank - 1) + "]", null);
		}

		private void AppendPointerType([NotNull] IPointerType pointerType, NamespaceDisplays expectedNamespaceDisplay) {
			AppendType(pointerType.ElementType, expectedNamespaceDisplay);
			AppendText("*", VsHighlightingAttributeIds.Operator);
		}

		[NotNull]
		private static string FormatShortName([NotNull] string shortName) {
			return CSharpLexer.IsKeyword(shortName) ? "@" + shortName : shortName;
		}
		
		private void AppendDeclaredType([NotNull] IDeclaredType declaredType, NamespaceDisplays expectedNamespaceDisplay) {
			if (declaredType.IsNullable()) {
				IType underlyingType = declaredType.GetNullableUnderlyingType();
				if (underlyingType != null) {
					AppendType(underlyingType, expectedNamespaceDisplay);
					AppendText("?", VsHighlightingAttributeIds.Operator);
					return;
				}
			}

			if (declaredType is IDynamicType) {
				AppendText("dynamic", VsHighlightingAttributeIds.Keyword);
				return;
			}

			if (_options.UseTypeKeywords) {
				string typeKeyword = CSharpTypeFactory.GetTypeKeyword(declaredType.GetClrName());
				if (typeKeyword != null) {
					AppendText(typeKeyword, VsHighlightingAttributeIds.Keyword);
					return;
				}
			}
			else if (declaredType.IsVoid()) {
				AppendText("void", VsHighlightingAttributeIds.Keyword);
				return;
			}

			ITypeElement typeElement = declaredType.GetTypeElement();
			if (typeElement == null || !typeElement.IsValid()) {
				AppendText(declaredType.GetPresentableName(UnknownLanguage.Instance), null);
				return;
			}

			AppendTypeElement(typeElement, declaredType.GetSubstitution(), expectedNamespaceDisplay);
		}

		private void AppendTypeElement([NotNull] ITypeElement typeElement, [NotNull] ISubstitution substitution, NamespaceDisplays expectedNamespaceDisplay) {
			if (!(typeElement is ITypeParameter)) {
				if ((_options.ShowNamespaces & expectedNamespaceDisplay) != NamespaceDisplays.None) {
					AppendNamespace(typeElement.GetContainingNamespace());
					AppendText(".", VsHighlightingAttributeIds.Operator);
				}

				ITypeElement containingType = typeElement.GetContainingType();
				if (containingType != null && !(typeElement is IDelegate && _options.FormatDelegatesAsLambdas)) {
					AppendDeclaredType(TypeFactory.CreateType(containingType, substitution), NamespaceDisplays.None);
					AppendText(".", VsHighlightingAttributeIds.Operator);
				}
			}

			var deleg = typeElement as IDelegate;
			if (deleg != null && _options.FormatDelegatesAsLambdas && expectedNamespaceDisplay == NamespaceDisplays.Parameters) {
				AppendParameters(deleg.InvokeMethod, substitution, false);
				AppendText(" => ", VsHighlightingAttributeIds.Operator);
				AppendType(substitution.Apply(deleg.InvokeMethod.ReturnType), expectedNamespaceDisplay);
				return;
			}

			string attributeId = _options.UseReSharperColors
				? HighlightingAttributeIds.GetHighlightAttributeForTypeElement(typeElement)
				: VsHighlightingAttributeIds.GetForTypeElement(typeElement);

			AppendText(FormatShortName(typeElement.ShortName), attributeId);
			AppendTypeParameters(typeElement, substitution);
		}

		private void AppendNamespace([NotNull] INamespace ns) {
			var containingNamespaces = new Stack<string>();
			for (INamespace iter = ns.GetContainingNamespace(); iter != null && !iter.IsRootNamespace; iter = iter.GetContainingNamespace())
				containingNamespaces.Push(iter.ShortName);

			string attributeId = _options.UseReSharperColors ? HighlightingAttributeIds.NAMESPACE_IDENTIFIER_ATTRIBUTE : null;
			
			while (containingNamespaces.Count > 0) {
				AppendText(containingNamespaces.Pop(), attributeId);
				AppendText(".", VsHighlightingAttributeIds.Operator);
			}
			AppendText(ns.ShortName, attributeId);
		}

		private void AppendTypeParameters([NotNull] ITypeElement typeElement, [NotNull] ISubstitution substitution) {
			IList<ITypeParameter> typeParameters = typeElement.TypeParameters;
			int typeParameterCount = typeParameters.Count;
			if (typeParameterCount == 0)
				return;

			AppendText("<", VsHighlightingAttributeIds.Operator);
			for (int i = 0; i < typeParameterCount; ++i) {
				if (i > 0)
					AppendText(",", null);
				ITypeParameter typeParameter = typeParameters[i];
				AppendType(substitution.Apply(typeParameter), NamespaceDisplays.TypeParameters);
			}
			AppendText(">", VsHighlightingAttributeIds.Operator);
		}

		private void AppendNameWithContainer([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, [CanBeNull] string highlightingAttributeId) {
			var typeElement = element as ITypeElement;
			if (typeElement != null) {
				AppendTypeElement(typeElement, substitution, NamespaceDisplays.Member);
				return;
			}

			var ns = element as INamespace;
			if (ns != null) {
				AppendNamespace(ns);
				return;
			}

			var typeMember = element as ITypeMember;
			if (typeMember != null) {
				ITypeElement containingTypeElement = typeMember.GetContainingType();
				if (containingTypeElement != null) {
					AppendTypeElement(containingTypeElement, substitution, NamespaceDisplays.Member);
					AppendText(".", VsHighlightingAttributeIds.Operator);
				}
			}
			
			AppendName(element, highlightingAttributeId);
		}
		
		private void AppendName([NotNull] IDeclaredElement element, [CanBeNull] string highlightingAttributeId) {
			if (CSharpDeclaredElementUtil.IsDestructor(element)) {
				ITypeElement containingType = ((IClrDeclaredElement) element).GetContainingType();
				if (containingType != null) {
					AppendText("~", VsHighlightingAttributeIds.Operator);
					AppendText(containingType.ShortName, highlightingAttributeId);
					return;
				}
			}
			
			if (CSharpDeclaredElementUtil.IsIndexer(element)) {
				AppendText("this", VsHighlightingAttributeIds.Keyword);
				return;
			}

			if (element is IAnonymousMethod) {
				AppendText("Anonymous method", highlightingAttributeId);
				return;
			}

			var conversionOperator = element as IConversionOperator;
			if (conversionOperator != null) {
				if (conversionOperator.IsImplicitCast)
					AppendText("implicit", VsHighlightingAttributeIds.Keyword);
				else if (conversionOperator.IsExplicitCast)
					AppendText("explicit", VsHighlightingAttributeIds.Keyword);
				return;
			}

			if (element is ISignOperator) {
				string signOperator = GetSignOperator(element.ShortName);
				if (!signOperator.IsEmpty()) {
					string attributeId = _options.UseReSharperColors ? HighlightingAttributeIds.OPERATOR_IDENTIFIER_ATTRIBUTE : VsHighlightingAttributeIds.Operator;
					AppendText(signOperator, attributeId);
					return;
				}
			}

			var ns = element as INamespace;
			if (ns != null && ns.IsRootNamespace) {
				AppendText("<Root Namespace>", highlightingAttributeId);
				return;
			}

			var parameter = element as IParameter;
			if (parameter != null && parameter.IsVarArg) {
				AppendText("__arglist", VsHighlightingAttributeIds.Keyword);
				return;
			}

			var constructor = element as IConstructor;
			if (constructor != null) {
				ITypeElement containingType = constructor.GetContainingType();
				string shortName = containingType != null ? containingType.ShortName : constructor.ShortName;
				AppendText(FormatShortName(shortName), highlightingAttributeId);
				return;
			}

			AppendText(FormatShortName(element.ShortName), highlightingAttributeId);
		}

		[CanBeNull]
		private static string GetSignOperator([CanBeNull] string operatorName) {
			switch (operatorName) {
				case "op_UnaryPlus":
					return "+";
				case "op_UnaryNegation":
					return "-";
				case "op_LogicalNot":
					return "!";
				case "op_OnesComplement":
					return "~";
				case "op_Increment":
					return "++";
				case "op_Decrement":
					return "--";
				case "op_True":
					return "true";
				case "op_False":
					return "false";
				case "op_Addition":
					return "+";
				case "op_Subtraction":
					return "-";
				case "op_Multiply":
					return "*";
				case "op_Division":
					return "/";
				case "op_Modulus":
					return "%";
				case "op_BitwiseAnd":
					return "&";
				case "op_BitwiseOr":
					return "|";
				case "op_ExclusiveOr":
					return "^";
				case "op_LeftShift":
					return "<<";
				case "op_RightShift":
					return ">>";
				case "op_Equality":
					return "==";
				case "op_Inequality":
					return "!=";
				case "op_LessThan":
					return "<";
				case "op_LessThanOrEqual":
					return "<=";
				case "op_GreaterThan":
					return ">";
				case "op_GreaterThanOrEqual":
					return ">=";
			}
			return null;
		}

		private void AppendParameters([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, bool isTopLevel) {
			var parametersOwner = TryGetParametersOwner(element);
			if (parametersOwner == null || !ShouldShowParameters(element))
				return;

			bool isIndexer = IsIndexer(element);
			AppendText(isIndexer ? "[" : "(", null);
			IList<IParameter> parameters = parametersOwner.Parameters;

			if (parameters.Count == 0 && _options.ShowEmptyParametersText) {
				if (isTopLevel)
					AppendText("<no parameters>", new TextStyle(FontStyle.Regular, Color.Gray));
			}

			else {
				for (int i = 0; i < parameters.Count; i++) {
					if (i > 0)
						AppendText(", ", null);
					int startOffset = _richText.Length;
					AppendParameter(parameters[i], substitution);
					if (isTopLevel)
						_presentedInfo.Parameters.Add(new TextRange(startOffset, _richText.Length));
				}
			}

			AppendText(isIndexer ? "]" : ")", null);
		}

		[CanBeNull]
		private static IParametersOwner TryGetParametersOwner([NotNull] IDeclaredElement declaredElement) {
			var parametersOwner = declaredElement as IParametersOwner;
			if (parametersOwner != null)
				return parametersOwner;

			var deleg = declaredElement as IDelegate;
			if (deleg != null)
				return deleg.InvokeMethod;

			return null;
		}

		private void AppendParameter([NotNull] IParameter parameter, [NotNull] ISubstitution substitution) {
			if (parameter.IsVarArg) {
				AppendText("__arglist", VsHighlightingAttributeIds.Keyword);
				return;
			}

			string modifier = GetParameterModifier(parameter);
			if (!modifier.IsEmpty()) {
				AppendText(modifier + " ", VsHighlightingAttributeIds.Keyword);
				if (modifier == "this")
					_presentedInfo.IsExtensionMethod = true;
			}

			if (_options.ShowParametersNullness)
				AppendNullness(parameter);

			if (_options.ShowParametersType)
				AppendElementType(parameter, substitution, NamespaceDisplays.Parameters, null, _options.ShowParametersName ? " " : null);

			if (_options.ShowParametersName) {
				string attributeId = _options.UseReSharperColors ? HighlightingAttributeIds.PARAMETER_IDENTIFIER_ATTRIBUTE : null;
				AppendText(FormatShortName(parameter.ShortName), attributeId);
			}

			if (_options.ShowConstantValues)
				AppendDefaultValue(parameter, substitution);
		}

		private void AppendNullness([NotNull] IAttributesOwner attributesOwner) {
			string nullnessName = GetNullnessName(attributesOwner);
			if (nullnessName == null)
				return;

			AppendText("[", null);
			AppendText(nullnessName, _options.UseReSharperColors ? HighlightingAttributeIds.TYPE_CLASS_ATTRIBUTE : VsHighlightingAttributeIds.Classes);
			AppendText("] ", null);
		}

		[CanBeNull]
		private string GetNullnessName([NotNull] IAttributesOwner attributesOwner) {
			CodeAnnotationNullableValue? nullableValue = _codeAnnotationsCache.GetNullableAttribute(attributesOwner);
			switch (nullableValue) {
				case CodeAnnotationNullableValue.NOT_NULL:
					return _notNullAttributeName;
				case CodeAnnotationNullableValue.CAN_BE_NULL:
					return _canBeNullAttributeName;
				default:
					return null;
			}
		}

		private static bool ShouldShowParameters([NotNull] IDeclaredElement element) {
			if (CSharpDeclaredElementUtil.IsDestructor(element) || CSharpDeclaredElementUtil.IsProperty(element))
				return false;
			
			var accessor = element as IAccessor;
			return accessor == null
				|| accessor.Kind != AccessorKind.GETTER
				|| !CSharpDeclaredElementUtil.IsProperty(accessor.OwnerMember);
		}

		private static bool IsIndexer([NotNull] IDeclaredElement element) {
			var accessor = element as IAccessor;
			if (accessor != null)
				element = accessor.OwnerMember;

			return CSharpDeclaredElementUtil.IsIndexer(element) || CSharpDeclaredElementUtil.IsIndexedProperty(element);
		}

		[CanBeNull]
		private static string GetParameterModifier([NotNull] IParameter parameter) {
			if (parameter.IsParameterArray)
				return "params";
			if (parameter.IsExtensionFirstParameter())
				return "this";

			switch (parameter.Kind) {
				case ParameterKind.REFERENCE:
					return "ref";
				case ParameterKind.OUTPUT:
					return "out";
				default:
					return null;
			}
		}

		private void AppendDefaultValue([NotNull] IParameter parameter, [NotNull] ISubstitution substitution) {
			if (!parameter.IsOptional)
				return;

			DefaultValue defaultValue = parameter.GetDefaultValue().Substitute(substitution).Normalize();
			if (defaultValue.IsBadValue) {
				AppendText(" = ", VsHighlightingAttributeIds.Operator);
				AppendText("bad value", null);
				return;
			}

			if (defaultValue.IsConstant) {
				AppendText(" = ", VsHighlightingAttributeIds.Operator);
				string presentation = defaultValue.ConstantValue.GetPresentation(CSharpLanguage.Instance);
				AppendText(presentation, CSharpLexer.IsKeyword(presentation) ? VsHighlightingAttributeIds.Keyword : null);
				return;
			}

			IType defaultType = defaultValue.DefaultTypeValue;
			if (defaultType == null)
				return;
			
			if (defaultType.IsNullable() || defaultType.IsReferenceType()) {
				AppendText(" = ", VsHighlightingAttributeIds.Operator);
				AppendText("null", VsHighlightingAttributeIds.Keyword);
				return;
			}

			AppendText(" = ", VsHighlightingAttributeIds.Operator);
			AppendText("default", VsHighlightingAttributeIds.Keyword);
			AppendText("(", null);
			AppendType(defaultType, NamespaceDisplays.Parameters);
			AppendText(")", null);
		}

		public CSharpColorizer([NotNull] RichText richText, [NotNull] PresenterOptions options,
			[NotNull] PresentedInfo presentedInfo, [NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache) {
			Debug.Assert(codeAnnotationsCache != null, "codeAnnotationsCache != null");

			_richText = richText;
			_options = options;
			_presentedInfo = presentedInfo;
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsCache = codeAnnotationsCache;
		}

	}

}