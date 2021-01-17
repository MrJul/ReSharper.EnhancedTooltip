using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
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

		private readonly struct Context {

			[NotNull] internal readonly PresenterOptions Options;
			[CanBeNull] internal readonly PresentedInfo PresentedInfo;
			[CanBeNull] internal readonly ITreeNode ContextualNode;

			public Context([NotNull] PresenterOptions options, [CanBeNull] PresentedInfo presentedInfo, [CanBeNull] ITreeNode contextualNode) {
				Options = options;
				PresentedInfo = presentedInfo;
				ContextualNode = contextualNode;
			}

		}

		private static readonly DeclaredElementPresenterStyle _specialTypeStyle = new DeclaredElementPresenterStyle(NameStyle.SHORT) {
			ShowTypeParameters = TypeParameterStyle.NONE
		};

		[NotNull] private readonly RichText _richText;
		[NotNull] private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
		[NotNull] private readonly CodeAnnotationsConfiguration _codeAnnotationsConfiguration;
		[NotNull] private readonly HighlighterIdProvider _highlighterIdProvider;

		[NotNull]
		public HighlighterIdProvider HighlighterIdProvider
			=> _highlighterIdProvider;

		public PresentedInfo AppendDeclaredElement(IDeclaredElement element, ISubstitution substitution, PresenterOptions options, ITreeNode contextualNode) {
			var context = new Context(options, new PresentedInfo(), contextualNode);

			if (!IsClrPresentableElement(element))
				return context.PresentedInfo;

			if (options.ShowElementKind != ElementKindDisplay.None)
				AppendElementKind(element, context, options.ShowElementKind == ElementKindDisplay.Stylized);

			if (options.ShowAccessRights)
				AppendAccessRights(element, true);

			if (options.ShowModifiers)
				AppendModifiers(element);

			if (element is IAttributesSet attributesSet)
				AppendAttributes(attributesSet, options.ShowElementAttributes, options.ShowElementAttributesArguments, options.AttributesFormattingMode, context);

			if (options.ShowElementType == ElementTypeDisplay.Before)
				AppendElementType(element, substitution, QualifierDisplays.ElementType, null, " ", context);

			if (options.ShowName)
				AppendNameWithContainer(element, substitution, context);

			if (options.ShowTypeParameters) {
				if (element is ITypeParametersOwner typeParametersOwner)
					AppendTypeParameters(typeParametersOwner, substitution, true, true, context);
			}

			if (options.ShowParametersType || options.ShowParametersName)
				AppendParameters(element, substitution, true, context);

			if (options.ShowAccessors) {
				if (element is IProperty property)
					AppendAccessors(property, context);
			}

			if (options.ShowElementType == ElementTypeDisplay.After)
				AppendElementType(element, substitution, QualifierDisplays.ElementType, ":", null, context);

			if (options.ShowConstantValue) {
				if (element is IConstantValueOwner constantValueOwner)
					AppendConstantValueOwner(constantValueOwner);
			}

			return context.PresentedInfo;
		}

		private static bool IsClrPresentableElement([NotNull] IDeclaredElement element)
			=> element.IsValid()
			&& element is IClrDeclaredElement
			&& element.GetElementType().IsPresentable(CSharpLanguage.Instance);

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

		private void AppendElementKind([CanBeNull] IDeclaredElement element, Context context, bool stylized) {
			PresenterOptions options = context.Options;
			string kind = element.GetElementKindString(
				options.UseExtensionMethodKind,
				options.UseAttributeClassKind,
				options.UseClassModifiersInKind,
				options.UseStructModifiersInKind,
				options.UseMethodModifiersInKind);
			AppendElementKind(kind, stylized);
		}

		private void AppendElementKind([NotNull] string kind, bool stylized) {
			if (stylized)
				AppendText("(" + kind + ") ", new TextStyle(FontStyle.Italic));
			else
				AppendText(kind + " ", null);
		}

		public void AppendAccessRights([NotNull] IDeclaredElement element, bool addSpaceAfter) {
			if (element is IAccessRightsOwner accessRightsOwner)
				AppendAccessRights(accessRightsOwner.GetAccessRights(), addSpaceAfter);
		}

		private void AppendAccessRights(AccessRights accessRights, bool addSpaceAfter) {
			string accessRightsString = CSharpDeclaredElementPresenter.Instance.Format(accessRights);
			if (accessRightsString.IsNullOrEmpty())
				return;

			AppendText(accessRightsString, _highlighterIdProvider.Keyword);

			if (addSpaceAfter)
				AppendText(" ", null);
		}

		private void AppendModifiers([NotNull] IDeclaredElement element) {
			if (!(element is IModifiersOwner modifiersOwner) || modifiersOwner is IAccessor || element.IsDestructor())
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
				AppendText(builder.ToString(), _highlighterIdProvider.Keyword);
		}

		private void AppendElementType(
			[NotNull] IDeclaredElement element,
			[NotNull] ISubstitution substitution,
			QualifierDisplays expectedQualifierDisplay,
			[CanBeNull] string before,
			[CanBeNull] string after,
			Context context) {

			// Use the special type first if available (eg Razor helper), colorize it as a keyword
			string specialTypeString = CSharpModificationUtil.GetSpecialElementType(_specialTypeStyle, element, substitution);
			if (!specialTypeString.IsEmpty()) {
				AppendText(before, null);
				AppendText(specialTypeString, _highlighterIdProvider.Keyword);
				AppendText(after, null);
				return;
			}

			IType elementType = GetElementType(element, substitution);
			if (elementType == null)
				return;

			AppendText(before, null);

			if (element.IsRefReturnMember() || (element is ICSharpLocalVariable localVariable && localVariable.ReferenceKind.IsByReference()))
				AppendText("ref ", _highlighterIdProvider.Keyword);
			else if (element is IParameter parameter) {
				string specialModifier = GetParameterSpecialModifier(parameter);
				if (!specialModifier.IsEmpty())
					AppendText(specialModifier, _highlighterIdProvider.Keyword);
				string kindModifier = GetParameterKindModifier(parameter);
				if (!kindModifier.IsEmpty())
					AppendText(kindModifier, _highlighterIdProvider.Keyword);
			}

			AppendType(elementType, expectedQualifierDisplay, true, context);
			AppendText(after, null);
		}

		[CanBeNull]
		private static IType GetElementType([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution) {
			switch (element) {
				case IConstructor _:
					return null;
				case var _ when element.IsDestructor():
					return null;
				case ITypeOwner typeOwner:
					return substitution.Apply(typeOwner.Type);
				case IParametersOwner parametersOwner:
					return substitution.Apply(parametersOwner.ReturnType);
				case IDelegate deleg:
					return substitution.Apply(deleg.InvokeMethod.ReturnType);
				default:
					return null;
			}
		}

		public void AppendExpressionType([CanBeNull] IExpressionType expressionType, bool appendModuleName, [NotNull] PresenterOptions options) {
			if (expressionType == null)
				return;

			IType itype = expressionType.ToIType();
			if (itype != null) {
				AppendType(itype, QualifierDisplays.Everywhere, true, new Context(options, null, null));
				if (appendModuleName)
					AppendModuleName(itype);
				return;
			}

			AppendText(expressionType.GetLongPresentableName(CSharpLanguage.Instance!), null);
		}

		private void AppendType([NotNull] IType type, QualifierDisplays expectedQualifierDisplay, bool displayUnknownTypeParameters, Context context) {
			switch (type) {
				case IArrayType arrayType:
					AppendArrayType(arrayType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
					return;
				case IPointerType pointerType:
					AppendPointerType(pointerType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
					return;
				case IDeclaredType declaredType:
					AppendDeclaredType(declaredType, expectedQualifierDisplay, true, displayUnknownTypeParameters, context);
					return;
				case IAnonymousType _:
					AppendText("anonymous type", null);
					return;
				default:
					AppendText(type.GetLongPresentableName(CSharpLanguage.Instance!), null);
					return;
			}
		}

		private void AppendModuleName([NotNull] IType itype) {
			AppendText(" [", null);
			AppendText(GetModuleFullName(itype), null);
			AppendText("]", null);
		}

		private void AppendArrayType([NotNull] IArrayType arrayType, QualifierDisplays expectedQualifierDisplay, bool displayUnknownTypeParameters, Context context) {
			AppendType(arrayType.ElementType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
			AppendText("[" + new string(',', arrayType.Rank - 1) + "]", null);
		}

		private void AppendPointerType([NotNull] IPointerType pointerType, QualifierDisplays expectedQualifierDisplay, bool displayUnknownTypeParameters, Context context) {
			AppendType(pointerType.ElementType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
			AppendText("*", _highlighterIdProvider.Operator);
		}

		[NotNull]
		private static string FormatShortName([NotNull] string shortName)
			=> CSharpLexer.IsKeyword(shortName) ? "@" + shortName : shortName;

		private void AppendDeclaredType(
			[NotNull] IDeclaredType declaredType,
			QualifierDisplays expectedQualifierDisplay,
			bool appendTypeParameters,
			bool displayUnknownTypeParameters,
			Context context) {

			if (declaredType is IDynamicType) {
				AppendText("dynamic", _highlighterIdProvider.Keyword);
				return;
			}

			if (declaredType is ITupleType tupleType) {
				AppendTupleType(tupleType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
				return;
			}

			if (context.Options.UseTypeKeywords) {
				string typeKeyword = CSharpTypeFactory.GetTypeKeyword(declaredType.GetClrName());
				if (typeKeyword != null) {
					AppendText(typeKeyword, _highlighterIdProvider.Keyword);
					if (declaredType.NullableAnnotation == NullableAnnotation.Annotated)
						AppendText("?", _highlighterIdProvider.Operator);
					return;
				}
			}
			else if (declaredType.IsVoid()) {
				AppendText("void", _highlighterIdProvider.Keyword);
				return;
			}

			ITypeElement typeElement = declaredType.GetTypeElement();
			if (typeElement == null || !typeElement.IsValid()) {
				AppendText(declaredType.GetPresentableName(CSharpLanguage.Instance!), null);
			}
			else {
				AppendTypeElement(typeElement, declaredType.GetSubstitution(), expectedQualifierDisplay, appendTypeParameters, displayUnknownTypeParameters, context);
				if (declaredType.NullableAnnotation == NullableAnnotation.Annotated && !Equals(typeElement.GetClrName(), PredefinedType.GENERIC_NULLABLE_FQN))
					AppendText("?", _highlighterIdProvider.Operator);
			}
		}

		private void AppendTupleType(
			[NotNull] ITupleType tupleType,
			QualifierDisplays expectedQualifierDisplay,
			bool displayUnknownTypeParameters,
			Context context) {

			AppendText("(", null);

			IReadOnlyList<TupleTypeComponent> components = tupleType.Components;
			int componentCount = components.Count;
			for (int i = 0; i < componentCount; ++i) {
				if (i > 0)
					AppendText(", ", null);

				TupleTypeComponent component = components[i];
				AppendType(component.Type, expectedQualifierDisplay, displayUnknownTypeParameters, context);
				if (component.HasExplicitName) {
					AppendText(" ", null);
					AppendText(component.ExplicitName, _highlighterIdProvider.TupleComponentName);
				}
			}

			AppendText(")", null);
		}

		private void AppendTypeElement(
			[NotNull] ITypeElement typeElement,
			[NotNull] ISubstitution substitution,
			QualifierDisplays expectedQualifierDisplay,
			bool appendTypeParameters,
			bool displayUnknownTypeParameters,
			Context context) {

			if (context.Options.UseShortNullableForm
			&& Equals(typeElement.GetClrName(), PredefinedType.GENERIC_NULLABLE_FQN)
			&& typeElement.TypeParameters.Count == 1) {
				IType underlyingType = substitution.Apply(typeElement.TypeParameters[0]);
				AppendType(underlyingType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
				AppendText("?", _highlighterIdProvider.Operator);
				return;
			}

			if (!(typeElement is ITypeParameter) && (context.Options.ShowQualifiers & expectedQualifierDisplay) != QualifierDisplays.None) {
				if (AppendNamespaceParts(GetNamespacePartsToDisplay(typeElement, context)))
					AppendText(".", _highlighterIdProvider.Operator);

				ITypeElement containingType = typeElement.GetContainingType();
				if (containingType != null && !(typeElement is IDelegate && context.Options.FormatDelegatesAsLambdas)) {
					AppendDeclaredType(TypeFactory.CreateType(containingType, substitution, NullableAnnotation.Unknown), QualifierDisplays.None, true, displayUnknownTypeParameters, context);
					AppendText(".", _highlighterIdProvider.Operator);
				}
			}

			if (typeElement is IDelegate deleg && context.Options.FormatDelegatesAsLambdas && expectedQualifierDisplay == QualifierDisplays.Parameters) {
				AppendParameters(deleg.InvokeMethod, substitution, false, context);
				AppendText(" => ", _highlighterIdProvider.Operator);
				AppendType(substitution.Apply(deleg.InvokeMethod.ReturnType), expectedQualifierDisplay, displayUnknownTypeParameters, context);
				return;
			}

			AppendText(FormatShortName(typeElement.ShortName), _highlighterIdProvider.GetForTypeElement(typeElement));

			if (appendTypeParameters)
				AppendTypeParameters(typeElement, substitution, false, displayUnknownTypeParameters, context);
		}

		[CanBeNull]
		private static List<string> GetNamespacePartsToDisplay([NotNull] ITypeElement typeElement, Context context) {
			if (typeElement is ICompiledElement) {
				switch (context.Options.ExternalCodeNamespaceDisplayKind) {

					case ExternalCodeNamespaceDisplayKind.Never:
						return null;

					case ExternalCodeNamespaceDisplayKind.Always:
						return GetNamespaceParts(typeElement.GetContainingNamespace());

					case ExternalCodeNamespaceDisplayKind.OnlyForNonSystem:
						if (typeElement.IsInSystemLikeNamespace())
							goto case ExternalCodeNamespaceDisplayKind.Never;
						goto case ExternalCodeNamespaceDisplayKind.Always;

					default:
						goto case ExternalCodeNamespaceDisplayKind.Always;
				}
			}

			switch (context.Options.SolutionCodeNamespaceDisplayKind) {

				case SolutionCodeNamespaceDisplayKind.Never:
					return null;

				case SolutionCodeNamespaceDisplayKind.Always:
					return GetNamespaceParts(typeElement.GetContainingNamespace());

				case SolutionCodeNamespaceDisplayKind.Smart:
					var contextualNamespace = context.ContextualNode?.GetContainingNode<INamespaceDeclaration>();
					if (contextualNamespace == null)
						goto case SolutionCodeNamespaceDisplayKind.Always;
					return GetDifferentNamespaceParts(typeElement.GetContainingNamespace(), contextualNamespace.DeclaredElement);

				default:
					goto case SolutionCodeNamespaceDisplayKind.Always;
			}
		}

		[NotNull]
		private static List<string> GetDifferentNamespaceParts([CanBeNull] INamespace sourceNamespace, [CanBeNull] INamespace contextualNamespace) {
			List<string> sourceParts = GetNamespaceParts(sourceNamespace);
			List<string> contextualParts = GetNamespaceParts(contextualNamespace);

			int length = Math.Min(sourceParts.Count, contextualParts.Count);
			int index = 0;
			while (index < length && sourceParts[index] == contextualParts[index])
				++index;

			if (index > 0) {
				sourceParts.RemoveRange(0, index);
				if (sourceParts.Count > 0)
					sourceParts.Insert(0, "…");
			}

			return sourceParts;
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

		private bool AppendNamespaceParts([CanBeNull] List<string> namespaceParts) {
			if (namespaceParts == null || namespaceParts.Count == 0)
				return false;

			string namespaceHighlighterId = _highlighterIdProvider.Namespace;
			string operatorHighlighterId = _highlighterIdProvider.Operator;

			for (int i = 0; i < namespaceParts.Count; ++i) {
				if (i > 0)
					AppendText(".", operatorHighlighterId);
				AppendText(namespaceParts[i], namespaceHighlighterId);
			}

			return true;
		}

		[NotNull]
		private static List<string> GetNamespaceParts([CanBeNull] INamespace ns) {
			var parts = new List<string>();
			for (INamespace iter = ns; iter != null && !iter.IsRootNamespace; iter = iter.GetContainingNamespace())
				parts.Add(iter.ShortName);
			parts.Reverse();
			return parts;
		}

		private void AppendTypeParameters(
			[NotNull] ITypeParametersOwner typeParametersOwner,
			[NotNull] ISubstitution substitution,
			bool isTopLevel,
			bool displayUnknownTypeParameters,
			Context context) {

			IList<ITypeParameter> typeParameters = typeParametersOwner.TypeParameters;
			int typeParameterCount = typeParameters.Count;
			if (typeParameterCount == 0)
				return;

			AppendText("<", _highlighterIdProvider.Operator);
			for (int i = 0; i < typeParameterCount; ++i) {
				if (i > 0)
					AppendText(",", null);

				int startOffset = _richText.Length;
				ITypeParameter typeParameter = typeParameters[i];

				if (context.Options.ShowTypeParametersVariance)
					AppendVariance(typeParameter.Variance);

				IType type = substitution.Apply(typeParameter);
				if (displayUnknownTypeParameters || !type.IsUnknown)
					AppendType(type, QualifierDisplays.TypeParameters, displayUnknownTypeParameters, context);

				if (isTopLevel)
					context.PresentedInfo?.TypeParameters.Add(new TextRange(startOffset, _richText.Length));
			}
			AppendText(">", _highlighterIdProvider.Operator);
		}

		private void AppendVariance(TypeParameterVariance variance) {
			switch (variance) {
				case TypeParameterVariance.IN:
					AppendText("in ", _highlighterIdProvider.Keyword);
					break;
				case TypeParameterVariance.OUT:
					AppendText("out ", _highlighterIdProvider.Keyword);
					break;
			}
		}

		private void AppendNameWithContainer([NotNull] IDeclaredElement element, [NotNull] ISubstitution substitution, Context context) {
			if (element is ITypeElement typeElement) {
				AppendTypeElement(typeElement, substitution, QualifierDisplays.Member, false, false, context);
				return;
			}

			if (element is INamespace ns) {
				AppendNamespaceParts(GetNamespaceParts(ns));
				return;
			}

			bool appendedExplicitInterface = false;
			if (context.Options.ShowExplicitInterface) {
				if (element is IOverridableMember overridableMember && overridableMember.IsExplicitImplementation) {
					IDeclaredType declaredType = CSharpDeclaredElementUtil.InterfaceQualification(overridableMember);
					if (declaredType != null) {
						AppendDeclaredType(declaredType, QualifierDisplays.None, true, true, context);
						AppendText(".", _highlighterIdProvider.Operator);
						appendedExplicitInterface = true;
					}
				}
			}

			if (!appendedExplicitInterface && (context.Options.ShowQualifiers & QualifierDisplays.Member) != QualifierDisplays.None) {
				ITypeElement containingTypeElement = (element as ITypeMember)?.GetContainingType();
				if (containingTypeElement != null) {
					AppendTypeElement(containingTypeElement, substitution, QualifierDisplays.Member, true, true, context);
					AppendText(".", _highlighterIdProvider.Operator);
				}
			}

			AppendName(element);
		}

		private void AppendName([NotNull] IDeclaredElement element) {
			string highlighterId = _highlighterIdProvider.GetForDeclaredElement(element);

			switch (element) {

				case var _ when element.IsDestructor():
					ITypeElement containingType = ((IClrDeclaredElement) element).GetContainingType();
					if (containingType != null) {
						AppendText("~", _highlighterIdProvider.Operator);
						AppendText(containingType.ShortName, highlighterId);
					}
					return;

				case var _ when CSharpDeclaredElementUtil.IsIndexer(element):
					AppendText("this", _highlighterIdProvider.Keyword);
					return;

				case IAnonymousMethod _:
					AppendText("Anonymous method", highlighterId);
					return;

				case IConversionOperator conversionOperator:
					if (conversionOperator.IsImplicitCast)
						AppendText("implicit", _highlighterIdProvider.Keyword);
					else if (conversionOperator.IsExplicitCast)
						AppendText("explicit", _highlighterIdProvider.Keyword);
					return;

				case ISignOperator _:
					AppendText(GetSignOperator(element.ShortName), highlighterId);
					return;

				case INamespace ns when ns.IsRootNamespace:
					AppendText("<Root Namespace>", highlighterId);
					return;

				case IParameter parameter when parameter.IsVarArg:
					AppendText("__arglist", _highlighterIdProvider.Keyword);
					return;

				case IConstructor constructor:
					string shortName = constructor.GetContainingType()?.ShortName ?? constructor.ShortName;
					AppendText(FormatShortName(shortName), highlighterId);
					return;

				default:
					AppendText(FormatShortName(element.ShortName), highlighterId);
					return;
			}

		}

		[NotNull]
		private static string GetSignOperator([NotNull] string operatorName) {
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

			int parameterCount = parameters.Count;
			if (parameterCount == 0) {
				if (isTopLevel && context.Options.ShowEmptyParametersText)
					AppendText("<no parameters>", new TextStyle(FontStyle.Regular, Color.Gray));
			}

			else {

				ParametersFormattingMode formattingMode = context.Options.ParametersFormattingMode;
				bool addNewLineAroundParameters = isTopLevel && formattingMode == ParametersFormattingMode.AllOnNewLine;
				bool addNewLineBeforeEachParameter = isTopLevel && ShouldAddNewLineBeforeEachParameter(formattingMode, parameterCount);

				if (addNewLineAroundParameters)
					AppendText("\r\n    ", null);

				for (int i = 0; i < parameterCount; i++) {
					if (i > 0)
						AppendText(", ", null);

					if (addNewLineBeforeEachParameter)
						AppendText("\r\n    ", null);

					int startOffset = _richText.Length;

					IParameter parameter = parameters[i];
					AppendParameter(parameter, substitution, context);

					if (isTopLevel && context.PresentedInfo != null) {
						context.PresentedInfo.Parameters.Add(new TextRange(startOffset, _richText.Length));
						if (parameter.IsExtensionFirstParameter())
							context.PresentedInfo.IsExtensionMethod = true;
					}
				}

				if (addNewLineAroundParameters || addNewLineBeforeEachParameter)
					AppendText("\r\n", null);
			}

			AppendText(isIndexer ? "]" : ")", null);
		}

		[Pure]
		private static bool ShouldAddNewLineBeforeEachParameter(ParametersFormattingMode formattingMode, int parameterCount) {
			switch (formattingMode) {
				case ParametersFormattingMode.AllOnCurrentLine:
					return false;
				case ParametersFormattingMode.AllOnNewLine:
					return false;
				case ParametersFormattingMode.OnePerLine:
					return true;
				case ParametersFormattingMode.OnePerLineIfMultiple:
					return parameterCount > 1;
				default:
					return false;
			}
		}

		[CanBeNull]
		private static IParametersOwner TryGetParametersOwner([NotNull] IDeclaredElement declaredElement) {
			if (declaredElement is IParametersOwner parametersOwner)
				return parametersOwner;

			return (declaredElement as IDelegate)?.InvokeMethod;
		}

		private void AppendParameter([NotNull] IParameter parameter, [NotNull] ISubstitution substitution, Context context) {
			if (parameter.IsVarArg) {
				AppendText("__arglist", _highlighterIdProvider.Keyword);
				return;
			}

			PresenterOptions options = context.Options;

			AppendAttributes(parameter, options.ShowParametersAttributes, options.ShowParametersAttributesArguments, AttributesFormattingMode.AllOnCurrentLine, context);

			if (options.ShowParametersType)
				AppendElementType(parameter, substitution, QualifierDisplays.Parameters, null, options.ShowParametersName ? " " : null, context);

			if (options.ShowParametersName)
				AppendText(FormatShortName(parameter.ShortName), _highlighterIdProvider.Parameter);

			if (options.ShowDefaultValues)
				AppendDefaultValue(parameter, substitution, context);
		}

		private void AppendAttributes(
			[NotNull] IAttributesSet attributesSet,
			AttributesDisplayKind displayKind,
			bool showArguments,
			AttributesFormattingMode formattingMode,
			Context context) {

			if (displayKind == AttributesDisplayKind.Never)
				return;

			IList<IAttributeInstance> attributes = attributesSet.GetAttributeInstances(false);
			if (attributes.Count == 0)
				return;

			var filteredAttributes = new LocalList<IAttributeInstance>();
			foreach (IAttributeInstance attribute in attributes) {
				if (MatchesAttributesDisplayKind(attribute, displayKind))
					filteredAttributes.Add(attribute);
			}

			if (filteredAttributes.Count == 0)
				return;

			bool addNewLineAroundAttributes = formattingMode == AttributesFormattingMode.AllOnNewLine;
			bool addNewLineBeforeEachAttribute = ShouldAddNewLineBeforeEachAttribute(formattingMode, filteredAttributes.Count);

			if (addNewLineAroundAttributes)
				AppendText("\r\n", null);

			foreach (IAttributeInstance attribute in filteredAttributes) {
				if (addNewLineBeforeEachAttribute)
					AppendText("\r\n", null);
				AppendAttribute(attribute, showArguments, context);
			}

			if (addNewLineAroundAttributes || addNewLineBeforeEachAttribute)
				AppendText("\r\n", null);
		}

		private bool MatchesAttributesDisplayKind([CanBeNull] IAttributeInstance attribute, AttributesDisplayKind displayKind) {
			if (attribute == null)
				return false;

			switch (displayKind) {
				case AttributesDisplayKind.Never:
					return false;
				case AttributesDisplayKind.NullnessAnnotations:
					return _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.NotNullAttributeShortName)
						|| _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.CanBeNullAttributeShortName);
				case AttributesDisplayKind.NullnessAndItemNullnessAnnotations:
					return _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.NotNullAttributeShortName)
						|| _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.CanBeNullAttributeShortName)
						|| _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, ContainerElementNullnessProvider.ItemNotNullAttributeShortName)
						|| _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, ContainerElementNullnessProvider.ItemCanBeNullAttributeShortName);
				case AttributesDisplayKind.AllAnnotations:
					return _codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, attribute.GetClrName().ShortName);
				case AttributesDisplayKind.Always:
					return true;
				default:
					return false;
			}
		}

		[Pure]
		private static bool ShouldAddNewLineBeforeEachAttribute(AttributesFormattingMode formattingMode, int attributeCount) {
			switch (formattingMode) {
				case AttributesFormattingMode.AllOnCurrentLine:
					return false;
				case AttributesFormattingMode.AllOnNewLine:
					return false;
				case AttributesFormattingMode.OnePerLine:
					return true;
				case AttributesFormattingMode.OnePerLineIfMultiple:
					return attributeCount > 1;
				default:
					return false;
			}
		}

		private void AppendAttribute([NotNull] IAttributeInstance attribute, bool showArguments, Context context) {
			AppendText("[", null);
			AppendText(attribute.GetClrName().ShortName.TrimFromEnd(AttributeInstanceExtensions.ATTRIBUTE_SUFFIX, StringComparison.Ordinal), _highlighterIdProvider.Class);

			if (showArguments) {

				bool hasParenthesis = false;
				foreach (AttributeValue attributeValue in attribute.PositionParameters()) {
					if (hasParenthesis)
						AppendText(", ", null);
					else {
						AppendText("(", null);
						hasParenthesis = true;
					}

					AppendAttributeValue(attributeValue, context);
				}

				foreach (Pair<string, AttributeValue> pair in attribute.NamedParameters()) {
					if (hasParenthesis)
						AppendText(", ", null);
					else {
						AppendText("(", null);
						hasParenthesis = true;
					}

					AppendText(pair.First, _highlighterIdProvider.Property);
					AppendText(" = ", _highlighterIdProvider.Operator);
					AppendAttributeValue(pair.Second, context);
				}

				if (hasParenthesis)
					AppendText(")", null);

			}

			AppendText("] ", null);
		}

		private void AppendAttributeValue([NotNull] AttributeValue attributeValue, Context context) {
			if (attributeValue.IsArray) {
				AppendText("new", _highlighterIdProvider.Keyword);
				AppendText(" { ", null);
				AttributeValue[] arrayValue = attributeValue.ArrayValue;
				if (arrayValue != null) {
					for (int i = 0; i < arrayValue.Length; ++i) {
						if (i > 0)
							AppendText(", ", null);
						AppendAttributeValue(arrayValue[i], context);
					}
				}
				AppendText(" }", null);
				return;
			}

			if (attributeValue.IsType) {
				AppendText("typeof", _highlighterIdProvider.Keyword);
				AppendText("(", null);
				IType typeValue = attributeValue.TypeValue;
				if (typeValue != null)
					AppendType(typeValue, QualifierDisplays.None, false, context);
				AppendText(")", null);
				return;
			}

			AppendConstantValue(attributeValue.ConstantValue, false);
		}

		private static bool ShouldShowParameters([NotNull] IDeclaredElement element) {
			if (element.IsDestructor() || CSharpDeclaredElementUtil.IsProperty(element))
				return false;

			return !(element is IAccessor accessor)
				|| accessor.Kind != AccessorKind.GETTER
				|| !CSharpDeclaredElementUtil.IsProperty(accessor.OwnerMember);
		}

		private static bool IsIndexer([NotNull] IDeclaredElement element) {
			if (element is IAccessor accessor)
				element = accessor.OwnerMember;

			return element.IsCSharpIndexer() || element.IsCSharpIndexedProperty();
		}

		[CanBeNull]
		private static string GetParameterSpecialModifier([NotNull] IParameter parameter) {
			if (parameter.IsParameterArray)
				return "params ";
			if (parameter.IsExtensionFirstParameter())
				return "this ";
			return null;
		}

		[CanBeNull]
		private static string GetParameterKindModifier([NotNull] IParameter parameter) {
			switch (parameter.Kind) {
				case ParameterKind.REFERENCE:
					return "ref ";
				case ParameterKind.OUTPUT:
					return "out ";
				case ParameterKind.INPUT:
					return "in ";
				default:
					return null;
			}
		}

		private void AppendDefaultValue([NotNull] IParameter parameter, [NotNull] ISubstitution substitution, Context context) {
			if (!parameter.IsOptional)
				return;

			DefaultValue defaultValue = parameter.GetDefaultValue().Substitute(substitution).Normalize();
			if (defaultValue.IsBadValue) {
				AppendText(" = ", _highlighterIdProvider.Operator);
				AppendText("bad value", null);
				return;
			}

			if (defaultValue.IsConstant) {
				AppendText(" = ", _highlighterIdProvider.Operator);
				AppendConstantValue(defaultValue.ConstantValue, false);
				return;
			}

			IType defaultType = defaultValue.DefaultTypeValue;
			if (defaultType == null)
				return;

			if (defaultType.IsNullable() || defaultType.IsReferenceType()) {
				AppendText(" = ", _highlighterIdProvider.Operator);
				AppendText("null", _highlighterIdProvider.Keyword);
				return;
			}

			AppendText(" = ", _highlighterIdProvider.Operator);
			AppendText("default", _highlighterIdProvider.Keyword);
			AppendText("(", null);
			AppendType(defaultType, QualifierDisplays.Parameters, true, context);
			AppendText(")", null);
		}

		private void AppendConstantValueOwner([NotNull] IConstantValueOwner constantValueOwner) {
			ConstantValue constantValue = constantValueOwner.ConstantValue;
			if (constantValue.IsBadValue())
				return;

			AppendText(" = ", _highlighterIdProvider.Operator);
			AppendConstantValue(constantValue, true);
		}

		private void AppendConstantValue([NotNull] ConstantValue constantValue, bool treatEnumAsIntegral) {
			if (constantValue.IsBadValue()) {
				AppendText("bad value", null);
				return;
			}

			IEnum enumType = constantValue.Type.GetEnumType();
			if (enumType != null) {
				if (treatEnumAsIntegral) {
					AppendText(constantValue.Value?.ToString() ?? String.Empty, _highlighterIdProvider.Number);
					return;
				}
				if (AppendEnumValue(constantValue, enumType))
					return;
			}

			string presentation = constantValue.GetPresentation(CSharpLanguage.Instance);
			if (CSharpLexer.IsKeyword(presentation)) {
				AppendText(presentation, _highlighterIdProvider.Keyword);
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
				AppendText(presentation, _highlighterIdProvider.String);
			else if (type.IsChar())
				AppendText(presentation, _highlighterIdProvider.String);
			else if (type.IsPredefinedNumeric())
				AppendText(presentation, _highlighterIdProvider.Number);
			else
				AppendText(presentation, null);
		}

		private bool AppendEnumValue([NotNull] ConstantValue constantValue, [NotNull] IEnum enumType) {
			IList<IField> fields = CSharpEnumUtil.CalculateEnumMembers(constantValue, enumType);
			if (fields.Count == 0)
				return false;

			string typeHighlighterId = _highlighterIdProvider.Enum;
			string valueHighlighterId = _highlighterIdProvider.Constant;

			var orderedFields = fields.OrderBy(f => f.ShortName);
			bool addSeparator = false;

			foreach (IField orderedField in orderedFields) {
				if (addSeparator)
					AppendText(" | ", _highlighterIdProvider.Operator);

				AppendText(enumType.ShortName, typeHighlighterId);
				AppendText(".", _highlighterIdProvider.Operator);
				AppendText(orderedField.ShortName, valueHighlighterId);

				addSeparator = true;
			}
			return true;
		}

		private void AppendAccessors([NotNull] IProperty property, Context context) {
			IAccessor getter = property.Getter;
			IAccessor setter = property.Setter;
			if (getter == null && setter == null)
				return;

			AppendText(" { ", null);
			AccessRights propertyAccessRights = property.GetAccessRights();
			if (getter != null)
				AppendAccessor(getter, "get", propertyAccessRights, context);
			if (setter != null)
				AppendAccessor(setter, "set", propertyAccessRights, context);
			AppendText("}", null);
		}

		private void AppendAccessor([NotNull] IAccessor accessor, [NotNull] string accessorName, AccessRights propertyAccessRights, Context context) {
			if (context.Options.ShowAccessRights) {
				AccessRights accessorAccessRights = accessor.GetAccessRights();
				if (accessorAccessRights != propertyAccessRights)
					AppendAccessRights(accessorAccessRights, true);
			}
			AppendText(accessorName, _highlighterIdProvider.Accessor);
			AppendText("; ", null);
		}

		public void AppendPresentableNode(ITreeNode treeNode, PresenterOptions options) {
			if (treeNode is ILiteralExpression literalExpression)
				AppendLiteralExpression(literalExpression, options);
			else if (treeNode is ITupleTypeComponent tupleTypeComponent)
				AppendTupleTypeComponent(tupleTypeComponent, options);
		}

		private void AppendLiteralExpression(ILiteralExpression literalExpression, PresenterOptions options) {
			var context = new Context(options, null, literalExpression);

			if (options.ShowElementKind != ElementKindDisplay.None)
				AppendElementKind("literal", options.ShowElementKind == ElementKindDisplay.Stylized);

			if (options.ShowElementType != ElementTypeDisplay.None)
				AppendType(literalExpression.Type(), QualifierDisplays.ElementType, true, context);

			if (options.ShowConstantValue)
				AppendConstantValueOwner(literalExpression);
		}

		private void AppendTupleTypeComponent([NotNull] ITupleTypeComponent tupleTypeComponent, [NotNull] PresenterOptions options) {
			if (!(tupleTypeComponent.Parent is ITupleTypeComponentList tupleComponentList))
				return;

			AppendElementKind("tuple component", options.ShowElementKind == ElementKindDisplay.Stylized);

			AppendText("(", null);

			var context = new Context(options, null, tupleTypeComponent);
			TreeNodeCollection<ITupleTypeComponent> components = tupleComponentList.Components;
			int componentCount = components.Count;
			for (int i = 0; i < componentCount; ++i) {
				if (i > 0)
					AppendText(", ", null);

				ITupleTypeComponent component = components[i];
				AppendType(CSharpTypeFactory.CreateType(component.TypeUsage), QualifierDisplays.None, true, context);
				ICSharpIdentifier nameIdentifier = component.NameIdentifier;
				if (nameIdentifier != null) {
					AppendText(" ", null);
					AppendText(nameIdentifier.Name, _highlighterIdProvider.TupleComponentName);
				}
			}

			AppendText(")", null);

			ICSharpIdentifier componentNameIdentifier = tupleTypeComponent.NameIdentifier;
			if (componentNameIdentifier != null) {
				AppendText(".", _highlighterIdProvider.Operator);
				AppendText(componentNameIdentifier.Name, _highlighterIdProvider.TupleComponentName);
			}
		}

		public CSharpColorizer(
			[NotNull] RichText richText,
			[NotNull] TextStyleHighlighterManager textStyleHighlighterManager,
			[NotNull] CodeAnnotationsConfiguration codeAnnotationsConfiguration,
			[NotNull] HighlighterIdProvider highlighterIdProvider) {
			_richText = richText;
			_textStyleHighlighterManager = textStyleHighlighterManager;
			_codeAnnotationsConfiguration = codeAnnotationsConfiguration;
			_highlighterIdProvider = highlighterIdProvider;
		}

	}

}