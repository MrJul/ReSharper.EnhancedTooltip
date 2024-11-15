using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Impl.Resolve;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Util.Media;
using JetBrains.Util.NetFX.Media.Colors;

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

      internal readonly PresenterOptions Options;
      internal readonly PresentedInfo? PresentedInfo;
      internal readonly ITreeNode? ContextualNode;

      public Context(PresenterOptions options, PresentedInfo? presentedInfo, ITreeNode? contextualNode) {
        this.Options = options;
        this.PresentedInfo = presentedInfo;
        this.ContextualNode = contextualNode;
      }

    }

    private static readonly DeclaredElementPresenterStyle _specialTypeStyle = new() {
      ShowTypeParameters = TypeParameterStyle.NONE,
      ShowName = NameStyle.SHORT
    };

    private readonly RichText _richText;
    private readonly TextStyleHighlighterManager _textStyleHighlighterManager;
    private readonly CodeAnnotationsConfiguration _codeAnnotationsConfiguration;
    private readonly HighlighterIdProvider _highlighterIdProvider;

    public HighlighterIdProvider HighlighterIdProvider
      => this._highlighterIdProvider;

    public PresentedInfo AppendDeclaredElement(IDeclaredElement element, ISubstitution substitution, PresenterOptions options, ITreeNode? contextualNode) {
      var context = new Context(options, new PresentedInfo(), contextualNode);

      if (!IsClrPresentableElement(element))
        return context.PresentedInfo!;

      if (options.ShowElementKind != ElementKindDisplay.None)
        this.AppendElementKind(element, context, options.ShowElementKind == ElementKindDisplay.Stylized);

      if (options.ShowAccessRights)
        this.AppendAccessRights(element, true);

      if (options.ShowModifiers)
        this.AppendModifiers(element);

      if (element is IAttributesSet attributesSet)
        this.AppendAttributes(attributesSet, options.ShowElementAttributes, options.ShowElementAttributesArguments, options.AttributesFormattingMode, context);

      if (options.ShowElementType == ElementTypeDisplay.Before)
        this.AppendElementType(element, substitution, QualifierDisplays.ElementType, null, " ", context);

      if (options.ShowName)
        this.AppendNameWithContainer(element, substitution, context);

      if (options.ShowTypeParameters) {
        if (element is ITypeParametersOwner typeParametersOwner)
          this.AppendTypeParameters(typeParametersOwner, substitution, true, true, context);
      }

      if (options.ShowParametersType || options.ShowParametersName)
        this.AppendParameters(element, substitution, true, context);

      if (options.ShowAccessors) {
        if (element is IProperty property)
          this.AppendAccessors(property, context);
      }

      if (options.ShowElementType == ElementTypeDisplay.After)
        this.AppendElementType(element, substitution, QualifierDisplays.ElementType, ":", null, context);

      if (options.ShowConstantValue) {
        if (element is IConstantValueOwner constantValueOwner)
          this.AppendConstantValueOwner(constantValueOwner);
      }

      return context.PresentedInfo!;
    }

    private static bool IsClrPresentableElement(IDeclaredElement element)
      => element.IsValid()
      && element is IClrDeclaredElement
      && element.GetElementType().IsPresentable(CSharpLanguage.Instance);

    public void AppendText(string? text, string? highlighterAttributeId) {
      if (text.IsEmpty())
        return;

      TextStyle textStyle = this._textStyleHighlighterManager.GetHighlighterTextStyle(highlighterAttributeId);
      this._richText.Append(text, textStyle);
    }

    private void AppendText(string? text, TextStyle textStyle) {
      if (!text.IsEmpty())
        this._richText.Append(text, textStyle);
    }

    private void AppendRichText(RichText text) {
      if (!text.IsEmpty)
        this._richText.Append(text);
    }

    private void AppendElementKind(IDeclaredElement? element, Context context, bool stylized) {
      PresenterOptions options = context.Options;
      string kind = element.GetElementKindString(
        options.UseExtensionMethodKind,
        options.UseAttributeClassKind,
        options.UseClassModifiersInKind,
        options.UseStructModifiersInKind,
        options.UseMethodModifiersInKind);
      this.AppendElementKind(kind, stylized);
    }

    private void AppendElementKind(string kind, bool stylized) {
      if (stylized)
        this.AppendText("(" + kind + ") ", new TextStyle(JetFontStyles.Italic));
      else
        this.AppendText(kind + " ", null);
    }

    public void AppendAccessRights(IDeclaredElement element, bool addSpaceAfter) {
      if (element is IAccessRightsOwner accessRightsOwner)
        this.AppendAccessRights(accessRightsOwner.GetAccessRights(), addSpaceAfter);
    }

    private void AppendAccessRights(AccessRights accessRights, bool addSpaceAfter) {
      if (accessRights == AccessRights.NONE) {
        return;
      }
      var accessRightsString = CSharpDeclaredElementPresenter.Instance.Format(accessRights);
      if (accessRightsString.IsNullOrEmpty())
        return;

      this.AppendText(accessRightsString, this._highlighterIdProvider.Keyword);

      if (addSpaceAfter)
        this.AppendText(" ", null);
    }

    private void AppendModifiers(IDeclaredElement element) {
      if (element is not IModifiersOwner modifiersOwner || modifiersOwner is IAccessor || element.IsDestructor())
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
      } else {
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
        this.AppendText(builder.ToString(), this._highlighterIdProvider.Keyword);
    }

    private void AppendElementType(
      IDeclaredElement element,
      ISubstitution substitution,
      QualifierDisplays expectedQualifierDisplay,
      string? before,
      string? after,
      Context context) {

      // Use the special type first if available (eg Razor helper), colorize it as a keyword
      string? specialTypeString = CSharpModificationUtil.GetSpecialElementType(_specialTypeStyle, element, substitution);
      if (!specialTypeString.IsEmpty()) {
        this.AppendText(before, null);
        this.AppendText(specialTypeString, this._highlighterIdProvider.Keyword);
        this.AppendText(after, null);
        return;
      }

      if (GetElementType(element, substitution) is not { } elementType)
        return;

      this.AppendText(before, null);

      if ((element.IsRefMember() || (element is ICSharpLocalVariable localVariable && localVariable.ReferenceKind.IsByReference())) && (element as IParameter)?.Kind == ParameterKind.REFERENCE)
        this.AppendText("ref ", this._highlighterIdProvider.Keyword);
      else if (element is IParameter parameter) {
        string? specialModifier = GetParameterSpecialModifier(parameter);
        if (!specialModifier.IsEmpty())
          this.AppendText(specialModifier, this._highlighterIdProvider.Keyword);
        string? kindModifier = GetParameterKindModifier(parameter);
        if (!kindModifier.IsEmpty())
          this.AppendText(kindModifier, this._highlighterIdProvider.Keyword);
      }

      this.AppendType(elementType, expectedQualifierDisplay, true, context);
      this.AppendText(after, null);
    }

    private static IType? GetElementType(IDeclaredElement element, ISubstitution substitution)
      => element switch {
        IConstructor => null,
        _ when element.IsDestructor() => null,
        ITypeOwner typeOwner => substitution.Apply(typeOwner.Type),
        IParametersOwner parametersOwner => substitution.Apply(parametersOwner.ReturnType),
        IDelegate deleg => substitution.Apply(deleg.InvokeMethod.ReturnType),
        _ => null
      };

    public void AppendExpressionType(IExpressionType? expressionType, bool appendModuleName, PresenterOptions options) {
      if (expressionType is null)
        return;

      if (expressionType.ToIType() is { } itype) {
        this.AppendType(itype, QualifierDisplays.Everywhere, true, new Context(options, null, null));
        if (appendModuleName)
          this.AppendModuleName(itype);
        return;
      }

      this.AppendText(expressionType.GetLongPresentableName(CSharpLanguage.Instance!), null);
    }

    private void AppendType(IType type, QualifierDisplays expectedQualifierDisplay, bool displayUnknownTypeParameters, Context context) {
      switch (type) {
        case IArrayType arrayType:
          this.AppendArrayType(arrayType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
          return;
        case IPointerType pointerType:
          this.AppendPointerType(pointerType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
          return;
        case IDeclaredType declaredType:
          this.AppendDeclaredType(declaredType, expectedQualifierDisplay, true, displayUnknownTypeParameters, context);
          return;
        case IAnonymousType:
          this.AppendText("anonymous type", null);
          return;
        default:
          this.AppendText(type.GetLongPresentableName(CSharpLanguage.Instance!), null);
          return;
      }
    }

    private void AppendModuleName(IType itype) {
      this.AppendText(" [", null);
      this.AppendText(GetModuleFullName(itype), null);
      this.AppendText("]", null);
    }

    private void AppendArrayType(IArrayType arrayType, QualifierDisplays expectedQualifierDisplay, bool displayUnknownTypeParameters, Context context) {
      this.AppendType(arrayType.ElementType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
      this.AppendText("[" + new string(',', arrayType.Rank - 1) + "]", null);
    }

    private void AppendPointerType(IPointerType pointerType, QualifierDisplays expectedQualifierDisplay, bool displayUnknownTypeParameters, Context context) {
      this.AppendType(pointerType.ElementType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
      this.AppendText("*", this._highlighterIdProvider.Operator);
    }

    private static string FormatShortName(string shortName)
      => CSharpLexer.IsKeyword(shortName) ? "@" + shortName : shortName;

    private void AppendDeclaredType(
      IDeclaredType declaredType,
      QualifierDisplays expectedQualifierDisplay,
      bool appendTypeParameters,
      bool displayUnknownTypeParameters,
      Context context) {

      if (declaredType.IsDynamicType()) {
        this.AppendText("dynamic", this._highlighterIdProvider.Keyword);
        return;
      }

      if (declaredType.IsTupleType()) {
        var tupleType = declaredType.AsTupleType();
        if (tupleType != null) {
          this.AppendTupleType(tupleType.Value, expectedQualifierDisplay, displayUnknownTypeParameters, context);
        }

        return;
      }

      if (context.Options.UseTypeKeywords) {
        if (CSharpTypeFactory.GetTypeKeyword(declaredType.GetTypeElement()) is { } typeKeyword) {
          this.AppendText(typeKeyword, this._highlighterIdProvider.Keyword);
          if (declaredType.NullableAnnotation == NullableAnnotation.Annotated)
            this.AppendText("?", this._highlighterIdProvider.Operator);
          return;
        }
      } else if (declaredType.IsVoid()) {
        this.AppendText("void", this._highlighterIdProvider.Keyword);
        return;
      }

      if (declaredType.GetTypeElement() is { } typeElement && typeElement.IsValid()) {
        this.AppendTypeElement(typeElement, declaredType.GetSubstitution(), expectedQualifierDisplay, appendTypeParameters, displayUnknownTypeParameters, context);
        if (declaredType.NullableAnnotation == NullableAnnotation.Annotated && !Equals(typeElement.GetClrName(), PredefinedType.GENERIC_NULLABLE_FQN))
          this.AppendText("?", this._highlighterIdProvider.Operator);
      } else {
        this.AppendText(declaredType.GetPresentableName(CSharpLanguage.Instance!), null);
      }
    }

    private void AppendTupleType(
      DecoratedType<TupleTypeDecoration> tupleType,
      QualifierDisplays expectedQualifierDisplay,
      bool displayUnknownTypeParameters,
      Context context) {

      this.AppendText("(", null);

      IReadOnlyList<TupleTypeComponent> components = tupleType.GetComponents();
      int componentCount = components.Count;
      for (int i = 0; i < componentCount; ++i) {
        if (i > 0)
          this.AppendText(", ", null);

        TupleTypeComponent component = components[i];
        this.AppendType(component.Type, expectedQualifierDisplay, displayUnknownTypeParameters, context);
        if (component.HasExplicitName) {
          this.AppendText(" ", null);
          this.AppendText(component.ExplicitName, this._highlighterIdProvider.TupleComponentName);
        }
      }

      this.AppendText(")", null);
    }

    private void AppendTypeElement(
      ITypeElement typeElement,
      ISubstitution substitution,
      QualifierDisplays expectedQualifierDisplay,
      bool appendTypeParameters,
      bool displayUnknownTypeParameters,
      Context context) {

      if (context.Options.UseShortNullableForm
      && Equals(typeElement.GetClrName(), PredefinedType.GENERIC_NULLABLE_FQN)
      && typeElement.TypeParameters.Count == 1) {
        IType underlyingType = substitution.Apply(typeElement.TypeParameters[0]);
        this.AppendType(underlyingType, expectedQualifierDisplay, displayUnknownTypeParameters, context);
        this.AppendText("?", this._highlighterIdProvider.Operator);
        return;
      }

      if (typeElement is not ITypeParameter && (context.Options.ShowQualifiers & expectedQualifierDisplay) != QualifierDisplays.None) {
        if (this.AppendNamespaceParts(GetNamespacePartsToDisplay(typeElement, context)))
          this.AppendText(".", this._highlighterIdProvider.Operator);

        if (typeElement.GetContainingType() is { } containingType && !(typeElement is IDelegate && context.Options.FormatDelegatesAsLambdas)) {
          this.AppendDeclaredType(TypeFactory.CreateType(containingType, substitution, NullableAnnotation.Unknown), QualifierDisplays.None, true, displayUnknownTypeParameters, context);
          this.AppendText(".", this._highlighterIdProvider.Operator);
        }
      }

      if (typeElement is IDelegate deleg && context.Options.FormatDelegatesAsLambdas && expectedQualifierDisplay == QualifierDisplays.Parameters) {
        this.AppendParameters(deleg.InvokeMethod, substitution, false, context);
        this.AppendText(" => ", this._highlighterIdProvider.Operator);
        this.AppendType(substitution.Apply(deleg.InvokeMethod.ReturnType), expectedQualifierDisplay, displayUnknownTypeParameters, context);
        return;
      }

      this.AppendText(FormatShortName(typeElement.ShortName), this._highlighterIdProvider.GetForTypeElement(typeElement));

      if (appendTypeParameters)
        this.AppendTypeParameters(typeElement, substitution, false, displayUnknownTypeParameters, context);
    }

    private static List<string>? GetNamespacePartsToDisplay(ITypeElement typeElement, Context context) {
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
          if (context.ContextualNode?.GetContainingNode<INamespaceDeclaration>() is { } contextualNamespace)
            return GetDifferentNamespaceParts(typeElement.GetContainingNamespace(), contextualNamespace.DeclaredElement);
          goto case SolutionCodeNamespaceDisplayKind.Always;

        default:
          goto case SolutionCodeNamespaceDisplayKind.Always;
      }
    }

    private static List<string> GetDifferentNamespaceParts(INamespace? sourceNamespace, INamespace? contextualNamespace) {
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

    private static string GetModuleFullName(IType type) {
      if (type.GetScalarType()?.Assembly is { } assembly)
        return assembly.FullName;

      if ((type.Module as IAssemblyPsiModule)?.Assembly.AssemblyName.FullName is { } name)
        return name;

      return type.Module.DisplayName;
    }

    private bool AppendNamespaceParts(List<string>? namespaceParts) {
      if (namespaceParts is null || namespaceParts.Count == 0)
        return false;

      string namespaceHighlighterId = this._highlighterIdProvider.Namespace;
      string operatorHighlighterId = this._highlighterIdProvider.Operator;

      for (int i = 0; i < namespaceParts.Count; ++i) {
        if (i > 0)
          this.AppendText(".", operatorHighlighterId);
        this.AppendText(namespaceParts[i], namespaceHighlighterId);
      }

      return true;
    }

    private static List<string> GetNamespaceParts(INamespace? ns) {
      var parts = new List<string>();
      for (INamespace? iter = ns; iter is { IsRootNamespace: false }; iter = iter.GetContainingNamespace())
        parts.Add(iter.ShortName);
      parts.Reverse();
      return parts;
    }

    private void AppendTypeParameters(
      ITypeParametersOwner typeParametersOwner,
      ISubstitution substitution,
      bool isTopLevel,
      bool displayUnknownTypeParameters,
      Context context) {

      IList<ITypeParameter> typeParameters = typeParametersOwner.TypeParameters;
      int typeParameterCount = typeParameters.Count;
      if (typeParameterCount == 0)
        return;

      this.AppendText("<", this._highlighterIdProvider.Operator);
      for (int i = 0; i < typeParameterCount; ++i) {
        if (i > 0)
          this.AppendText(",", null);

        int startOffset = this._richText.Length;
        ITypeParameter typeParameter = typeParameters[i];

        if (context.Options.ShowTypeParametersVariance)
          this.AppendVariance(typeParameter.Variance);

        IType type = substitution.Apply(typeParameter);
        if (displayUnknownTypeParameters || !type.IsUnknown)
          this.AppendType(type, QualifierDisplays.TypeParameters, displayUnknownTypeParameters, context);

        if (isTopLevel)
          context.PresentedInfo?.TypeParameters.Add(new TextRange(startOffset, this._richText.Length));
      }

      this.AppendText(">", this._highlighterIdProvider.Operator);
    }

    private void AppendVariance(TypeParameterVariance variance) {
      switch (variance) {
        case TypeParameterVariance.IN:
          this.AppendText("in ", this._highlighterIdProvider.Keyword);
          break;
        case TypeParameterVariance.OUT:
          this.AppendText("out ", this._highlighterIdProvider.Keyword);
          break;
      }
    }

    private void AppendNameWithContainer(IDeclaredElement element, ISubstitution substitution, Context context) {
      if (element is ITypeElement typeElement) {
        this.AppendTypeElement(typeElement, substitution, QualifierDisplays.Member, false, false, context);
        return;
      }

      if (element is INamespace ns) {
        this.AppendNamespaceParts(GetNamespaceParts(ns));
        return;
      }

      bool appendedExplicitInterface = false;
      if (context.Options.ShowExplicitInterface
      && element is IOverridableMember { IsExplicitImplementation: true } overridableMember
      && CSharpDeclaredElementUtil.InterfaceQualification(overridableMember) is { } declaredType) {
        this.AppendDeclaredType(declaredType, QualifierDisplays.None, true, true, context);
        this.AppendText(".", this._highlighterIdProvider.Operator);
        appendedExplicitInterface = true;
      }

      if (!appendedExplicitInterface && (context.Options.ShowQualifiers & QualifierDisplays.Member) != QualifierDisplays.None
      && (element as ITypeMember)?.GetContainingType() is { } containingTypeElement) {
        this.AppendTypeElement(containingTypeElement, substitution, QualifierDisplays.Member, true, true, context);
        this.AppendText(".", this._highlighterIdProvider.Operator);
      }

      this.AppendName(element);
    }

    private void AppendName(IDeclaredElement element) {
      string? highlighterId = this._highlighterIdProvider.GetForDeclaredElement(element);

      switch (element) {

        case var _ when element.IsDestructor():
          if (((IClrDeclaredElement)element).GetContainingType() is { } containingType) {
            this.AppendText("~", this._highlighterIdProvider.Operator);
            this.AppendText(containingType.ShortName, highlighterId);
          }
          return;

        case var _ when CSharpDeclaredElementUtil.IsIndexer(element):
          this.AppendText("this", this._highlighterIdProvider.Keyword);
          return;

        case IAnonymousMethod:
          this.AppendText("Anonymous method", highlighterId);
          return;

        case IConversionOperator conversionOperator:
          if (conversionOperator.IsImplicitCast)
            this.AppendText("implicit", this._highlighterIdProvider.Keyword);
          else if (conversionOperator.IsExplicitCast)
            this.AppendText("explicit", this._highlighterIdProvider.Keyword);
          return;

        case ISignOperator:
          this.AppendText(GetSignOperator(element.ShortName), highlighterId);
          return;

        case INamespace { IsRootNamespace: true }:
          this.AppendText("<Root Namespace>", highlighterId);
          return;

        case IParameter { IsVarArg: true }:
          this.AppendText("__arglist", this._highlighterIdProvider.Keyword);
          return;

        case IConstructor constructor:
          string shortName = constructor.GetContainingType()?.ShortName ?? constructor.ShortName;
          this.AppendText(FormatShortName(shortName), highlighterId);
          return;

        default:
          this.AppendText(FormatShortName(element.ShortName), highlighterId);
          return;
      }
    }

    private static string GetSignOperator(string operatorName)
      => operatorName switch {
        OperatorName.UNARY_PLUS => "+",
        OperatorName.UNARY_MINUS => "-",
        OperatorName.UNARY_LOGNOT => "!",
        OperatorName.UNARY_COMPLEMENT => "~",
        OperatorName.UNARY_INCREMENT => "++",
        OperatorName.UNARY_DECREMENT => "--",
        OperatorName.UNARY_TRUE => "true",
        OperatorName.UNARY_FALSE => "false",
        OperatorName.BINARY_PLUS => "+",
        OperatorName.BINARY_MINUS => "-",
        OperatorName.BINARY_MULTIPLY => "*",
        OperatorName.BINARY_DIVIDE => "/",
        OperatorName.BINARY_MODULUS => "%",
        OperatorName.BINARY_BITWISE_AND => "&",
        OperatorName.BINARY_BITWISE_OR => "|",
        OperatorName.BINARY_EXCLUSIVE_OR => "^",
        OperatorName.BINARY_LEFTSHIFT => "<<",
        OperatorName.BINARY_RIGHTSHIFT => ">>",
        OperatorName.BINARY_EQUALITY => "==",
        OperatorName.BINARY_INEQUALITY => "!=",
        OperatorName.BINARY_LT => "<",
        OperatorName.BINARY_LE => "<=",
        OperatorName.BINARY_GT => ">",
        OperatorName.BINARY_GE => ">=",
        _ => operatorName
      };

    private void AppendParameters(IDeclaredElement element, ISubstitution substitution, bool isTopLevel, Context context) {
      if (TryGetParametersOwner(element) is not { } parametersOwner || !ShouldShowParameters(element))
        return;

      bool isIndexer = IsIndexer(element);
      this.AppendText(isIndexer ? "[" : "(", null);
      IList<IParameter> parameters = parametersOwner.Parameters;

      int parameterCount = parameters.Count;
      if (parameterCount == 0) {
        if (isTopLevel && context.Options.ShowEmptyParametersText)
          this.AppendText("<no parameters>", new TextStyle(JetFontStyles.Regular, Color.Gray.ToJetRgbaColor()));
      } else {
        ParametersFormattingMode formattingMode = context.Options.ParametersFormattingMode;
        bool addNewLineAroundParameters = isTopLevel && formattingMode == ParametersFormattingMode.AllOnNewLine;
        bool addNewLineBeforeEachParameter = isTopLevel && ShouldAddNewLineBeforeEachParameter(formattingMode, parameterCount);

        if (addNewLineAroundParameters)
          this.AppendText("\r\n    ", null);

        for (int i = 0; i < parameterCount; i++) {
          if (i > 0)
            this.AppendText(", ", null);

          if (addNewLineBeforeEachParameter)
            this.AppendText("\r\n    ", null);

          int startOffset = this._richText.Length;

          IParameter parameter = parameters[i];
          this.AppendParameter(parameter, substitution, context);

          if (isTopLevel && context.PresentedInfo is not null) {
            context.PresentedInfo.Parameters.Add(new TextRange(startOffset, this._richText.Length));
            if (parameter.IsExtensionFirstParameter())
              context.PresentedInfo.IsExtensionMethod = true;
          }
        }

        if (addNewLineAroundParameters || addNewLineBeforeEachParameter)
          this.AppendText("\r\n", null);
      }

      this.AppendText(isIndexer ? "]" : ")", null);
    }

    [Pure]
    private static bool ShouldAddNewLineBeforeEachParameter(ParametersFormattingMode formattingMode, int parameterCount)
      => formattingMode switch {
        ParametersFormattingMode.AllOnCurrentLine => false,
        ParametersFormattingMode.AllOnNewLine => false,
        ParametersFormattingMode.OnePerLine => true,
        ParametersFormattingMode.OnePerLineIfMultiple => parameterCount > 1,
        _ => false
      };

    private static IParametersOwner? TryGetParametersOwner(IDeclaredElement declaredElement) {
      if (declaredElement is IParametersOwner parametersOwner)
        return parametersOwner;

      return (declaredElement as IDelegate)?.InvokeMethod;
    }

    private void AppendParameter(IParameter parameter, ISubstitution substitution, Context context) {
      if (parameter.IsVarArg) {
        this.AppendText("__arglist", this._highlighterIdProvider.Keyword);
        return;
      }

      PresenterOptions options = context.Options;

      this.AppendAttributes(parameter, options.ShowParametersAttributes, options.ShowParametersAttributesArguments, AttributesFormattingMode.AllOnCurrentLine, context);

      if (options.ShowParametersType)
        this.AppendElementType(parameter, substitution, QualifierDisplays.Parameters, null, options.ShowParametersName ? " " : null, context);

      if (options.ShowParametersName)
        this.AppendText(FormatShortName(parameter.ShortName), this._highlighterIdProvider.Parameter);

      if (options.ShowDefaultValues)
        this.AppendDefaultValue(parameter, substitution, context);
    }

    private void AppendAttributes(
      IAttributesSet attributesSet,
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
        if (this.MatchesAttributesDisplayKind(attribute, displayKind))
          filteredAttributes.Add(attribute);
      }

      if (filteredAttributes.Count == 0)
        return;

      bool addNewLineAroundAttributes = formattingMode == AttributesFormattingMode.AllOnNewLine;
      bool addNewLineBeforeEachAttribute = ShouldAddNewLineBeforeEachAttribute(formattingMode, filteredAttributes.Count);

      if (addNewLineAroundAttributes)
        this.AppendText("\r\n", null);

      foreach (IAttributeInstance attribute in filteredAttributes) {
        if (addNewLineBeforeEachAttribute)
          this.AppendText("\r\n", null);
        this.AppendAttribute(attribute, showArguments, context);
      }

      if (addNewLineAroundAttributes || addNewLineBeforeEachAttribute)
        this.AppendText("\r\n", null);
    }

    private bool MatchesAttributesDisplayKind(IAttributeInstance? attribute, AttributesDisplayKind displayKind) {
      if (attribute is null)
        return false;

      return displayKind switch {
        AttributesDisplayKind.Never
          => false,
        AttributesDisplayKind.NullnessAnnotations
          => this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.NotNullAttributeShortName)
          || this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.CanBeNullAttributeShortName),
        AttributesDisplayKind.NullnessAndItemNullnessAnnotations
          => this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.NotNullAttributeShortName)
          || this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, NullnessProvider.CanBeNullAttributeShortName)
          || this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, ContainerElementNullnessProvider.ItemNotNullAttributeShortName)
          || this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, ContainerElementNullnessProvider.ItemCanBeNullAttributeShortName),
        AttributesDisplayKind.AllAnnotations
          => this._codeAnnotationsConfiguration.IsAnnotationAttribute(attribute, attribute.GetClrName().ShortName),
        AttributesDisplayKind.Always
          => true,
        _
          => false
      };
    }

    [Pure]
    private static bool ShouldAddNewLineBeforeEachAttribute(AttributesFormattingMode formattingMode, int attributeCount)
      => formattingMode switch {
        AttributesFormattingMode.AllOnCurrentLine => false,
        AttributesFormattingMode.AllOnNewLine => false,
        AttributesFormattingMode.OnePerLine => true,
        AttributesFormattingMode.OnePerLineIfMultiple => attributeCount > 1,
        _ => false
      };

    private void AppendAttribute(IAttributeInstance attribute, bool showArguments, Context context) {
      this.AppendText("[", null);
      this.AppendText(attribute.GetClrName().ShortName.TrimFromEnd(AttributeInstanceExtensions.ATTRIBUTE_SUFFIX, StringComparison.Ordinal), this._highlighterIdProvider.Class);

      if (showArguments) {

        bool hasParenthesis = false;
        foreach (AttributeValue attributeValue in attribute.PositionParameters()) {
          if (hasParenthesis)
            this.AppendText(", ", null);
          else {
            this.AppendText("(", null);
            hasParenthesis = true;
          }

          this.AppendAttributeValue(attributeValue, context);
        }

        foreach (Pair<string, AttributeValue> pair in attribute.NamedParameters()) {
          if (hasParenthesis)
            this.AppendText(", ", null);
          else {
            this.AppendText("(", null);
            hasParenthesis = true;
          }

          this.AppendText(pair.First, this._highlighterIdProvider.Property);
          this.AppendText(" = ", this._highlighterIdProvider.Operator);
          this.AppendAttributeValue(pair.Second, context);
        }

        if (hasParenthesis)
          this.AppendText(")", null);

      }

      this.AppendText("] ", null);
    }

    private void AppendAttributeValue(AttributeValue attributeValue, Context context) {
      if (attributeValue.IsArray) {
        this.AppendText("new", this._highlighterIdProvider.Keyword);
        this.AppendText(" { ", null);
        if (attributeValue.ArrayValue is { } arrayValue) {
          for (int i = 0; i < arrayValue.Length; ++i) {
            if (i > 0)
              this.AppendText(", ", null);
            this.AppendAttributeValue(arrayValue[i], context);
          }
        }

        this.AppendText(" }", null);
        return;
      }

      if (attributeValue.IsType) {
        this.AppendText("typeof", this._highlighterIdProvider.Keyword);
        this.AppendText("(", null);
        if (attributeValue.TypeValue is { } typeValue)
          this.AppendType(typeValue, QualifierDisplays.None, false, context);
        this.AppendText(")", null);
        return;
      }

      this.AppendConstantValue(attributeValue.ConstantValue, false);
    }

    private static bool ShouldShowParameters(IDeclaredElement element) {
      if (element.IsDestructor() || CSharpDeclaredElementUtil.IsProperty(element))
        return false;

      return element is not IAccessor { Kind: AccessorKind.GETTER } accessor
        || !CSharpDeclaredElementUtil.IsProperty(accessor.OwnerMember);
    }

    private static bool IsIndexer(IDeclaredElement? element) {
      if (element is IAccessor accessor)
        element = accessor.OwnerMember;

      return element.IsCSharpIndexer() || element.IsCSharpIndexedProperty();
    }

    private static string? GetParameterSpecialModifier(IParameter parameter) {
      if (parameter.IsParameterArray)
        return "params ";
      if (parameter.IsExtensionFirstParameter())
        return "this ";
      return null;
    }

    private static string? GetParameterKindModifier(IParameter parameter)
      => parameter.Kind switch {
        ParameterKind.REFERENCE => "ref ",
        ParameterKind.OUTPUT => "out ",
        ParameterKind.INPUT => "in ",
        _ => null
      };

    private void AppendDefaultValue(IParameter parameter, ISubstitution substitution, Context context) {
      if (!parameter.IsOptional)
        return;

      DefaultValue defaultValue = parameter.GetDefaultValue().Substitute(substitution).Normalize();
      if (defaultValue.IsBadValue) {
        this.AppendText(" = ", this._highlighterIdProvider.Operator);
        this.AppendText("bad value", null);
        return;
      }

      if (defaultValue.IsConstant) {
        this.AppendText(" = ", this._highlighterIdProvider.Operator);
        this.AppendConstantValue(defaultValue.ConstantValue, false);
        return;
      }

      if (defaultValue.DefaultTypeValue is not { } defaultType)
        return;

      if (defaultType.IsNullable() || defaultType.IsReferenceType()) {
        this.AppendText(" = ", this._highlighterIdProvider.Operator);
        this.AppendText("null", this._highlighterIdProvider.Keyword);
        return;
      }

      this.AppendText(" = ", this._highlighterIdProvider.Operator);
      this.AppendText("default", this._highlighterIdProvider.Keyword);
      this.AppendText("(", null);
      this.AppendType(defaultType, QualifierDisplays.Parameters, true, context);
      this.AppendText(")", null);
    }

    private void AppendConstantValueOwner(IConstantValueOwner constantValueOwner) {
      ConstantValue constantValue = constantValueOwner.ConstantValue;
      if (constantValue.IsBadValue())
        return;

      this.AppendText(" = ", this._highlighterIdProvider.Operator);
      this.AppendConstantValue(constantValue, true);
    }

    private void AppendConstantValue(ConstantValue constantValue, bool treatEnumAsIntegral) {
      if (constantValue.IsBadValue()) {
        this.AppendText("bad value", null);
        return;
      }

      if (constantValue.Type.GetEnumType() is { } enumType) {
        if (treatEnumAsIntegral) {
          this.AppendText(constantValue.IsEnum() ? constantValue.ToIntUnchecked().ToString() : constantValue.StringValue ?? String.Empty, this._highlighterIdProvider.Number);
          return;
        }
        if (this.AppendEnumValue(constantValue, enumType))
          return;
      }

      string presentation = constantValue.GetPresentation(CSharpLanguage.Instance, TypePresentationStyle.Default).Text;
      if (CSharpLexer.IsKeyword(presentation)) {
        this.AppendText(presentation, this._highlighterIdProvider.Keyword);
        return;
      }

      IType? type = constantValue.Type;
      if (type.IsNullable())
        type = type.GetNullableUnderlyingType();

      if (type is null) {
        this.AppendText(presentation, null);
        return;
      }

      if (type.IsString())
        this.AppendText(presentation, this._highlighterIdProvider.String);
      else if (type.IsChar())
        this.AppendText(presentation, this._highlighterIdProvider.String);
      else if (type.IsPredefinedNumeric())
        this.AppendText(presentation, this._highlighterIdProvider.Number);
      else
        this.AppendText(presentation, null);
    }

    private bool AppendEnumValue(ConstantValue constantValue, IEnum enumType) {
      IList<IField> fields = CSharpEnumUtil.CalculateEnumMembers(constantValue, enumType);
      if (fields.Count == 0)
        return false;

      string typeHighlighterId = this._highlighterIdProvider.Enum;
      string valueHighlighterId = this._highlighterIdProvider.Constant;

      var orderedFields = fields.OrderBy(f => f.ShortName);
      bool addSeparator = false;

      foreach (IField orderedField in orderedFields) {
        if (addSeparator)
          this.AppendText(" | ", this._highlighterIdProvider.Operator);

        this.AppendText(enumType.ShortName, typeHighlighterId);
        this.AppendText(".", this._highlighterIdProvider.Operator);
        this.AppendText(orderedField.ShortName, valueHighlighterId);

        addSeparator = true;
      }
      return true;
    }

    private void AppendAccessors(IProperty property, Context context) {
      IAccessor? getter = property.Getter;
      IAccessor? setter = property.Setter;
      if (getter is null && setter is null)
        return;

      this.AppendText(" { ", null);
      AccessRights propertyAccessRights = property.GetAccessRights();
      if (getter is not null)
        this.AppendAccessor(getter, "get", propertyAccessRights, context);
      if (setter is not null)
        this.AppendAccessor(setter, setter.IsInitOnly ? "init" : "set", propertyAccessRights, context);
      this.AppendText("}", null);
    }

    private void AppendAccessor(IAccessor accessor, string accessorName, AccessRights propertyAccessRights, Context context) {
      if (context.Options.ShowAccessRights) {
        AccessRights accessorAccessRights = accessor.GetAccessRights();
        if (accessorAccessRights != propertyAccessRights)
          this.AppendAccessRights(accessorAccessRights, true);
      }

      this.AppendText(accessorName, this._highlighterIdProvider.Accessor);
      this.AppendText("; ", null);
    }

    public void AppendPresentableNode(ITreeNode treeNode, PresenterOptions options) {
      if (treeNode is ILiteralExpression literalExpression)
        this.AppendLiteralExpression(literalExpression, options);
      else if (treeNode is ITupleTypeComponent tupleTypeComponent)
        this.AppendTupleTypeComponent(tupleTypeComponent, options);
    }

    private void AppendLiteralExpression(ILiteralExpression literalExpression, PresenterOptions options) {
      var context = new Context(options, null, literalExpression);

      if (options.ShowElementKind != ElementKindDisplay.None)
        this.AppendElementKind("literal", options.ShowElementKind == ElementKindDisplay.Stylized);

      if (options.ShowElementType != ElementTypeDisplay.None)
        this.AppendType(literalExpression.Type(), QualifierDisplays.ElementType, true, context);

      if (options.ShowConstantValue)
        this.AppendConstantValueOwner(literalExpression);
    }

    private void AppendTupleTypeComponent(ITupleTypeComponent tupleTypeComponent, PresenterOptions options) {
      if (tupleTypeComponent.Parent is not ITupleTypeComponentList tupleComponentList)
        return;

      this.AppendElementKind("tuple component", options.ShowElementKind == ElementKindDisplay.Stylized);

      this.AppendText("(", null);

      var context = new Context(options, null, tupleTypeComponent);
      var components = tupleComponentList.Components;
      int componentCount = components.Count;
      for (int i = 0; i < componentCount; ++i) {
        if (i > 0)
          this.AppendText(", ", null);

        ITupleTypeComponent component = components[i];
        if (component is not null) {
          this.AppendType(CSharpTypeFactory.CreateType(component.TypeUsage), QualifierDisplays.None, true, context);
          if (component.NameIdentifier is { } nameIdentifier) {
            this.AppendText(" ", null);
            this.AppendText(nameIdentifier.Name, this._highlighterIdProvider.TupleComponentName);
          }
        }
      }

      this.AppendText(")", null);

      if (tupleTypeComponent.NameIdentifier is { } componentNameIdentifier) {
        this.AppendText(".", this._highlighterIdProvider.Operator);
        this.AppendText(componentNameIdentifier.Name, this._highlighterIdProvider.TupleComponentName);
      }
    }

    public CSharpColorizer(
      RichText richText,
      TextStyleHighlighterManager textStyleHighlighterManager,
      CodeAnnotationsConfiguration codeAnnotationsConfiguration,
      HighlighterIdProvider highlighterIdProvider) {
      this._richText = richText;
      this._textStyleHighlighterManager = textStyleHighlighterManager;
      this._codeAnnotationsConfiguration = codeAnnotationsConfiguration;
      this._highlighterIdProvider = highlighterIdProvider;
    }

  }

}