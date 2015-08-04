using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using JetBrains.Annotations;
using JetBrains.Metadata.Utils;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
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

		private struct Context {

			[NotNull] internal readonly PresenterOptions Options;
			[CanBeNull] internal readonly PresentedInfo PresentedInfo;

			public Context([NotNull] PresenterOptions options, [CanBeNull] PresentedInfo presentedInfo) {
				Options = options;
				PresentedInfo = presentedInfo;
			}

		}

		private static readonly DeclaredElementPresenterStyle _specialTypeStyle = new DeclaredElementPresenterStyle(NameStyle.SHORT) {
			ShowTypeParameters = TypeParameterStyle.NONE
		};

		[NotNull] private readonly RichText _richText;
		[NotNull] private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		[NotNull] private readonly CodeAnnotationsCache _codeAnnotationsCache;

		public bool UseReSharperColors { get; }

		public PresentedInfo AppendDeclaredElement(IDeclaredElement element, ISubstitution substitution, PresenterOptions options) {
			var context = new Context(options, new PresentedInfo());

			if (!IsClrPresentableElement(element))
				return context.PresentedInfo;

			if (options.ShowElementKind)
				AppendElementKindStylized(element);

			if (options.ShowAccessRights)
				AppendAccessRights(element, true);
			if (options.ShowModifiers)
				AppendModifiers(element);

			var attributesSet = element as IAttributesSet;
			if (attributesSet != null)
				AppendAnnotations(attributesSet, options.ShowElementAnnotations);

			if (options.ShowElementType == ElementTypeDisplay.Before)
				AppendElementType(element, substitution, QualifierDisplays.ElementType, null, " ", context);

			if (options.ShowName)
				AppendNameWithContainer(element, substitution, context);
			if (options.ShowParametersType || options.ShowParametersName)
				AppendParameters(element, substitution, true, context);

			if (options.ShowElementType == ElementTypeDisplay.After)
				AppendElementType(element, substitution, QualifierDisplays.ElementType, ":", null, context);

			if (options.ShowConstantValue) {
				var constantValueOwner = element as IConstantValueOwner;
				if (constantValueOwner != null)
					AppendConstantValue(constantValueOwner);
			}

			return context.PresentedInfo;
		}

		private static bool IsClrPresentableElement([NotNull] IDeclaredElement element) {
			return element.IsValid()
				&& element is IClrDeclaredElement
				&& element.GetElementType().IsPresentable(CSharpLanguage.Instance);
		}

		public void AppendText([CanBeNull] string text, [CanBeNull] string highlighterAttributeId) {
			if (text.IsEmpty())
				return;

			TextStyle textStyle = _textStyleHighlighterManager.GetHighlighterTextStyle(highlighterAttributeId);
			_richText.Append(text, textStyle);
		}

		private void AppendText([CanBeNull] string text, TextStyle textStyle) {
			if (!text.IsEmpty())
				_richText.Append(text, textStyle);
		}
		
		private void AppendElementKindStylized([CanBeNull] IDeclaredElement element) {
			AppendText("(" + element.GetElementKindString() + ") ", new TextStyle(FontStyle.Italic));
		}

		public void AppendAccessRights([NotNull] IDeclaredElement element, bool addSpaceAfter) {
			var accessRightsOwner = element as IAccessRightsOwner;
			if (accessRightsOwner == null)
				return;

			string accessRights = CSharpDeclaredElementPresenter.Instance.Format(accessRightsOwner.GetAccessRights());
			if (accessRights.IsEmpty())
				return;

			AppendText(accessRights, VsHighlightingAttributeIds.Keyword);
			if (addSpaceAfter)
				AppendText(" ", null);
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

		private void AppendElementType([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, QualifierDisplays expectedQualifierDisplay,
			[CanBeNull] string before, [CanBeNull] string after, Context context) {

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
			AppendTypeWithoutModule(elementType, expectedQualifierDisplay, context);
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

		public void AppendExpressionType([CanBeNull] IExpressionType expressionType, bool appendModuleName, [NotNull] PresenterOptions options) {
			if (expressionType == null)
				return;

			IType itype = expressionType.ToIType();
			if (itype != null) {
				AppendTypeWithoutModule(itype, QualifierDisplays.Everywhere, new Context(options, null));
				if (appendModuleName)
					AppendModuleName(itype);
				return;
			}

			AppendText(expressionType.GetLongPresentableName(CSharpLanguage.Instance), null);
		}

		private void AppendTypeWithoutModule([NotNull] IType type, QualifierDisplays expectedQualifierDisplay, Context context) {
			var arrayType = type as IArrayType;
			if (arrayType != null) {
				AppendArrayType(arrayType, expectedQualifierDisplay, context);
				return;
			}

			var pointerType = type as IPointerType;
			if (pointerType != null) {
				AppendPointerType(pointerType, expectedQualifierDisplay, context);
				return;
			}

			var declaredType = type as IDeclaredType;
			if (declaredType != null) {
				AppendDeclaredType(declaredType, expectedQualifierDisplay, context);
				return;
			}

			if (type is IAnonymousType) {
				AppendText("anonymous type", null);
				return;
			}

			AppendText(type.GetLongPresentableName(CSharpLanguage.Instance), null);
		}

		private void AppendModuleName([NotNull] IType itype) {
			AppendText(" [", null);
			AppendText(GetModuleFullName(itype), null);
			AppendText("]", null);
		}

		private void AppendArrayType([NotNull] IArrayType arrayType, QualifierDisplays expectedQualifierDisplay, Context context) {
			AppendTypeWithoutModule(arrayType.ElementType, expectedQualifierDisplay, context);
			AppendText("[" + new string(',', arrayType.Rank - 1) + "]", null);
		}

		private void AppendPointerType([NotNull] IPointerType pointerType, QualifierDisplays expectedQualifierDisplay, Context context) {
			AppendTypeWithoutModule(pointerType.ElementType, expectedQualifierDisplay, context);
			AppendText("*", VsHighlightingAttributeIds.Operator);
		}

		[NotNull]
		private static string FormatShortName([NotNull] string shortName) {
			return CSharpLexer.IsKeyword(shortName) ? "@" + shortName : shortName;
		}
		
		private void AppendDeclaredType([NotNull] IDeclaredType declaredType, QualifierDisplays expectedQualifierDisplay, Context context) {
			if (declaredType.IsNullable()) {
				IType underlyingType = declaredType.GetNullableUnderlyingType();
				if (underlyingType != null) {
					AppendTypeWithoutModule(underlyingType, expectedQualifierDisplay, context);
					AppendText("?", VsHighlightingAttributeIds.Operator);
					return;
				}
			}

			if (declaredType is IDynamicType) {
				AppendText("dynamic", VsHighlightingAttributeIds.Keyword);
				return;
			}

			if (context.Options.UseTypeKeywords) {
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
				AppendText(declaredType.GetPresentableName(CSharpLanguage.Instance), null);
			}
			else
				AppendTypeElement(typeElement, declaredType.GetSubstitution(), expectedQualifierDisplay, context);
		}

		private void AppendTypeElement([NotNull] ITypeElement typeElement, [NotNull] ISubstitution substitution, QualifierDisplays expectedQualifierDisplay, Context context) {

			if (!(typeElement is ITypeParameter) && (context.Options.ShowQualifiers & expectedQualifierDisplay) != QualifierDisplays.None) {
				INamespace containingNamespace = typeElement.GetContainingNamespace();
				AppendNamespace(containingNamespace);
				if (!containingNamespace.IsRootNamespace)
					AppendText(".", VsHighlightingAttributeIds.Operator);

				ITypeElement containingType = typeElement.GetContainingType();
				if (containingType != null && !(typeElement is IDelegate && context.Options.FormatDelegatesAsLambdas)) {
					AppendDeclaredType(TypeFactory.CreateType(containingType, substitution), QualifierDisplays.None, context);
					AppendText(".", VsHighlightingAttributeIds.Operator);
				}
			}

			var deleg = typeElement as IDelegate;
			if (deleg != null && context.Options.FormatDelegatesAsLambdas && expectedQualifierDisplay == QualifierDisplays.Parameters) {
				AppendParameters(deleg.InvokeMethod, substitution, false, context);
				AppendText(" => ", VsHighlightingAttributeIds.Operator);
				AppendTypeWithoutModule(substitution.Apply(deleg.InvokeMethod.ReturnType), expectedQualifierDisplay, context);
				return;
			}

			string highlighterId = typeElement.GetHighlightingAttributeId(UseReSharperColors);
			AppendText(FormatShortName(typeElement.ShortName), highlighterId);
			AppendTypeParameters(typeElement, substitution, context);
		}

		[NotNull]
		private static string GetModuleFullName([NotNull] IType type) {
			AssemblyNameInfo assembly = type.GetScalarType()?.Assembly;
			if (assembly != null)
				return assembly.FullName;

			string name = (type.Module as IAssemblyPsiModule)?.Assembly.AssemblyName.FullName;
			if (name != null)
				return name;

			return type.Module.DisplayName;
		}

		private void AppendNamespace([NotNull] INamespace ns) {
			var containingNamespaces = new Stack<string>();
			for (INamespace iter = ns.GetContainingNamespace(); iter != null && !iter.IsRootNamespace; iter = iter.GetContainingNamespace())
				containingNamespaces.Push(iter.ShortName);

			string highlighterId = UseReSharperColors
				? HighlightingAttributeIds.NAMESPACE_IDENTIFIER_ATTRIBUTE
				: VsHighlightingAttributeIds.Identifier;
			
			while (containingNamespaces.Count > 0) {
				AppendText(containingNamespaces.Pop(), highlighterId);
				AppendText(".", VsHighlightingAttributeIds.Operator);
			}
			AppendText(ns.ShortName, highlighterId);
		}

		private void AppendTypeParameters([NotNull] ITypeElement typeElement, [NotNull] ISubstitution substitution, Context context) {
			IList<ITypeParameter> typeParameters = typeElement.TypeParameters;
			int typeParameterCount = typeParameters.Count;
			if (typeParameterCount == 0)
				return;

			AppendText("<", VsHighlightingAttributeIds.Operator);
			for (int i = 0; i < typeParameterCount; ++i) {
				if (i > 0)
					AppendText(",", null);
				ITypeParameter typeParameter = typeParameters[i];
				AppendTypeWithoutModule(substitution.Apply(typeParameter), QualifierDisplays.TypeParameters, context);
			}
			AppendText(">", VsHighlightingAttributeIds.Operator);
		}

		private void AppendNameWithContainer([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, Context context) {
			var typeElement = element as ITypeElement;
			if (typeElement != null) {
				AppendTypeElement(typeElement, substitution, QualifierDisplays.Member, context);
				return;
			}

			var ns = element as INamespace;
			if (ns != null) {
				AppendNamespace(ns);
				return;
			}

			bool appendedExplicitInterface = false;
			if (context.Options.ShowExplicitInterface) {
				var overridableMember = element as IOverridableMember;
				if (overridableMember != null && overridableMember.IsExplicitImplementation) {
					IDeclaredType declaredType = CSharpDeclaredElementUtil.InterfaceQualification(overridableMember);
					if (declaredType != null) {
						AppendDeclaredType(declaredType, QualifierDisplays.None, context);
						AppendText(".", VsHighlightingAttributeIds.Operator);
						appendedExplicitInterface = true;
					}
				}
			}

			if (!appendedExplicitInterface && (context.Options.ShowQualifiers & QualifierDisplays.Member) != QualifierDisplays.None) {
				ITypeElement containingTypeElement = (element as ITypeMember)?.GetContainingType();
				if (containingTypeElement != null) {
					AppendTypeElement(containingTypeElement, substitution, QualifierDisplays.Member, context);
					AppendText(".", VsHighlightingAttributeIds.Operator);
				}
			}

			AppendName(element);
		}

		private void AppendName([NotNull] IDeclaredElement element) {
			string highlighterId = element.GetHighlightingAttributeId(UseReSharperColors);

			if (CSharpDeclaredElementUtil.IsDestructor(element)) {
				ITypeElement containingType = ((IClrDeclaredElement) element).GetContainingType();
				if (containingType != null) {
					AppendText("~", VsHighlightingAttributeIds.Operator);
					AppendText(containingType.ShortName, highlighterId);
					return;
				}
			}
			
			if (CSharpDeclaredElementUtil.IsIndexer(element)) {
				AppendText("this", VsHighlightingAttributeIds.Keyword);
				return;
			}

			if (element is IAnonymousMethod) {
				AppendText("Anonymous method", highlighterId);
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
					AppendText(signOperator, highlighterId);
					return;
				}
			}

			var ns = element as INamespace;
			if (ns != null && ns.IsRootNamespace) {
				AppendText("<Root Namespace>", highlighterId);
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
				string shortName = containingType?.ShortName ?? constructor.ShortName;
				AppendText(FormatShortName(shortName), highlighterId);
				return;
			}

			AppendText(FormatShortName(element.ShortName), highlighterId);
		}

		[CanBeNull]
		private static string GetSignOperator([CanBeNull] string operatorName) {
			switch (operatorName) {
				case OperatorName.UNARY_PLUS: return "+";
				case OperatorName.UNARY_MINUS: return "-";
				case OperatorName.UNARY_LOGNOT: return "!";
				case OperatorName.UNARY_COMPLEMENT: return "~";
				case OperatorName.UNARY_INCREMENT: return "++";
				case OperatorName.UNARY_DECREMENT: return "--";
				case OperatorName.UNARY_TRUE: return "true";
				case OperatorName.UNARY_FALSE: return "false";
				case OperatorName.BINARY_PLUS: return "+";
				case OperatorName.BINARY_MINUS: return "-";
				case OperatorName.BINARY_MULTIPLY: return "*";
				case OperatorName.BINARY_DIVIDE: return "/";
				case OperatorName.BINARY_MODULUS: return "%";
				case OperatorName.BINARY_BITWISE_AND: return "&";
				case OperatorName.BINARY_BITWISE_OR: return "|";
				case OperatorName.BINARY_EXCLUSIVE_OR: return "^";
				case OperatorName.BINARY_LEFTSHIFT: return "<<";
				case OperatorName.BINARY_RIGHTSHIFT: return ">>";
				case OperatorName.BINARY_EQUALITY: return "==";
				case OperatorName.BINARY_INEQUALITY: return "!=";
				case OperatorName.BINARY_LT: return "<";
				case OperatorName.BINARY_LE: return "<=";
				case OperatorName.BINARY_GT: return ">";
				case OperatorName.BINARY_GE: return ">=";
				default: return operatorName;
			}
		}

		private void AppendParameters([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, bool isTopLevel, Context context) {
			var parametersOwner = TryGetParametersOwner(element);
			if (parametersOwner == null || !ShouldShowParameters(element))
				return;

			bool isIndexer = IsIndexer(element);
			AppendText(isIndexer ? "[" : "(", null);
			IList<IParameter> parameters = parametersOwner.Parameters;

			if (parameters.Count == 0 && context.Options.ShowEmptyParametersText) {
				if (isTopLevel)
					AppendText("<no parameters>", new TextStyle(FontStyle.Regular, Color.Gray));
			}

			else {
				for (int i = 0; i < parameters.Count; i++) {
					if (i > 0)
						AppendText(", ", null);
					int startOffset = _richText.Length;
					AppendParameter(parameters[i], substitution, context);
					if (isTopLevel && context.PresentedInfo != null)
						context.PresentedInfo.Parameters.Add(new TextRange(startOffset, _richText.Length));
				}
			}

			AppendText(isIndexer ? "]" : ")", null);
		}

		[CanBeNull]
		private static IParametersOwner TryGetParametersOwner([NotNull] IDeclaredElement declaredElement) {
			var parametersOwner = declaredElement as IParametersOwner;
			if (parametersOwner != null)
				return parametersOwner;

			return (declaredElement as IDelegate)?.InvokeMethod;
		}

		private void AppendParameter([NotNull] IParameter parameter, [NotNull] ISubstitution substitution, Context context) {
			if (parameter.IsVarArg) {
				AppendText("__arglist", VsHighlightingAttributeIds.Keyword);
				return;
			}

			string modifier = GetParameterModifier(parameter);
			if (!modifier.IsEmpty()) {
				AppendText(modifier + " ", VsHighlightingAttributeIds.Keyword);
				if (context.PresentedInfo != null && modifier == "this")
					context.PresentedInfo.IsExtensionMethod = true;
			}

			PresenterOptions options = context.Options;

			AppendAnnotations(parameter, options.ShowParametersAnnotations);
			
			if (options.ShowParametersType)
				AppendElementType(parameter, substitution, QualifierDisplays.Parameters, null, options.ShowParametersName ? " " : null, context);

			if (options.ShowParametersName) {
				string highlighterId = UseReSharperColors
					? HighlightingAttributeIds.PARAMETER_IDENTIFIER_ATTRIBUTE
					: VsHighlightingAttributeIds.Identifier;
				AppendText(FormatShortName(parameter.ShortName), highlighterId);
			}

			if (options.ShowDefaultValues)
				AppendDefaultValue(parameter, substitution, context);
		}

		private void AppendAnnotations([NotNull] IAttributesSet attributesSet, AnnotationsDisplayKind showAnnotations) {
			if (showAnnotations == AnnotationsDisplayKind.None)
				return;

			IList<IAttributeInstance> attributes = attributesSet.GetAttributeInstances(false);
			if (attributes.Count == 0)
				return;

			List<string> annotations = attributes
				.SelectNotNull(attr => TryGetAnnotationShortName(attr, showAnnotations))
				.Distinct()
				.OrderBy(annotation => annotation)
				.ToList();
			if (annotations.Count == 0)
				return;

			string highlighterId = UseReSharperColors
				? HighlightingAttributeIds.TYPE_CLASS_ATTRIBUTE
				: VsHighlightingAttributeIds.Classes;
			AppendText("[", null);
			for (int i = 0; i < annotations.Count; i++) {
				if (i > 0)
					AppendText(", ", null);
				AppendText(annotations[i], highlighterId);
			}
			AppendText("] ", null);
		}

		[CanBeNull]
		private string TryGetAnnotationShortName([CanBeNull] IAttributeInstance attribute, AnnotationsDisplayKind showAnnotations) {
			if (attribute != null && showAnnotations != AnnotationsDisplayKind.None) {
				string shortName = attribute.GetClrName().ShortName;
				if (IsDisplayedAnnotation(attribute, shortName, showAnnotations))
					return shortName.TrimFromEnd("Attribute");
			}
			return null;
		}

		private bool IsDisplayedAnnotation([CanBeNull] IAttributeInstance attribute, [CanBeNull] string shortName, AnnotationsDisplayKind showAnnotations) {
			if (attribute == null || shortName == null)
				return false;

			switch (showAnnotations) {
				case AnnotationsDisplayKind.Nullness:
					return shortName == CodeAnnotationsCache.CanBeNullAttributeShortName
						|| shortName == CodeAnnotationsCache.NotNullAttributeShortName;
				case AnnotationsDisplayKind.All:
					return _codeAnnotationsCache.IsAnnotationAttribute(attribute, shortName);
				default:
					return false;
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

			return CSharpDeclaredElementUtil.IsIndexer(element) || element.IsIndexedProperty();
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

		private void AppendDefaultValue([NotNull] IParameter parameter, [NotNull] ISubstitution substitution, Context context) {
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
				AppendConstantValue(defaultValue.ConstantValue);
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
			AppendTypeWithoutModule(defaultType, QualifierDisplays.Parameters, context);
			AppendText(")", null);
		}

		private void AppendConstantValue([NotNull] IConstantValueOwner constantValueOwner) {
			ConstantValue constantValue = constantValueOwner.ConstantValue;
			if (constantValue.IsBadValue())
				return;

			AppendText(" = ", VsHighlightingAttributeIds.Operator);
			AppendConstantValue(constantValue);
		}

		private void AppendConstantValue([NotNull] ConstantValue constantValue) {
			if (constantValue.IsBadValue()) {
				AppendText("bad value", null);
				return;
			}

			IEnum enumType = constantValue.Type.GetEnumType();
			if (enumType != null && AppendEnumValue(constantValue, enumType))
				return;

			string presentation = constantValue.GetPresentation(CSharpLanguage.Instance);
			if (presentation != null && CSharpLexer.IsKeyword(presentation)) {
				AppendText(presentation, VsHighlightingAttributeIds.Keyword);
				return;
			}

			IType type = constantValue.Type;
			if (type.IsNullable())
				type = type.GetNullableUnderlyingType();

			if (type == null) {
				AppendText(presentation, null);
				return;
			}
			
			if (type.IsString())
				AppendText(presentation, VsHighlightingAttributeIds.String);
			else if (type.IsChar())
				AppendText(presentation, VsHighlightingAttributeIds.String);
			else if (type.IsPredefinedNumeric())
				AppendText(presentation, VsHighlightingAttributeIds.Number);
			else
				AppendText(presentation, null);
		}

		private bool AppendEnumValue([NotNull] ConstantValue constantValue, [NotNull] IEnum enumType) {
			IList<IField> fields = CSharpEnumUtil.CalculateEnumMembers(constantValue, enumType);
			if (fields.Count == 0)
				return false;

			string typeHighlighterId = UseReSharperColors ? HighlightingAttributeIds.TYPE_ENUM_ATTRIBUTE : VsHighlightingAttributeIds.Enums;
			string valueHighlighterId = UseReSharperColors ? HighlightingAttributeIds.CONSTANT_IDENTIFIER_ATTRIBUTE : VsHighlightingAttributeIds.Identifier;
			
			var orderedFields = fields.OrderBy(f => f.ShortName);
			bool addSeparator = false;
			
			foreach (IField orderedField in orderedFields) {
				if (addSeparator)
					AppendText(" | ", VsHighlightingAttributeIds.Operator);

				AppendText(enumType.ShortName, typeHighlighterId);
				AppendText(".", VsHighlightingAttributeIds.Operator);
				AppendText(orderedField.ShortName, valueHighlighterId);

				addSeparator = true;
			}
			return true;
		}

		public CSharpColorizer([NotNull] RichText richText, [NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsCache codeAnnotationsCache, bool useReSharperColors) {
			_richText = richText;
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsCache = codeAnnotationsCache;
			UseReSharperColors = useReSharperColors;
		}

	}

}