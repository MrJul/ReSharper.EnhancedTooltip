using System.Linq.Expressions;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// Provides colored identifier tooltips.
	/// </summary>
	[SolutionComponent]
	public class IdentifierTooltipContentProvider {

		private sealed class DeclaredElementInfo {

			internal readonly IDeclaredElement DeclaredElement;
			internal readonly ISubstitution Substitution;
			internal readonly ITreeNode TreeNode;
			internal readonly TextRange SourceRange;
			internal readonly IReference? Reference;

			public DeclaredElementInfo(
				IDeclaredElement declaredElement,
				ISubstitution substitution,
				ITreeNode treeNode,
				TextRange sourceRange,
				IReference? reference) {
				DeclaredElement = declaredElement;
				Substitution = substitution;
				TreeNode = treeNode;
				SourceRange = sourceRange;
				Reference = reference;
			}

		}

		private readonly struct PresentableInfo {

			internal readonly DeclaredElementInfo? DeclaredElementInfo;
			internal readonly PresentableNode PresentableNode;

			public bool IsValid()
				=> DeclaredElementInfo is not null && DeclaredElementInfo.DeclaredElement.IsValid()
				|| PresentableNode.Node is not null;

			public PresentableInfo(DeclaredElementInfo? declaredElementInfo) {
				DeclaredElementInfo = declaredElementInfo;
				PresentableNode = default;
			}

			public PresentableInfo(PresentableNode presentableNode) {
				DeclaredElementInfo = null;
				PresentableNode = presentableNode;
			}

		}

		private readonly ISolution _solution;
		private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;
		private readonly ColorizerPresenter _colorizerPresenter;

		/// <summary>
		/// Returns a colored <see cref="IdentifierContentGroup"/> for an identifier represented by a <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter representing the identifier.</param>
		/// <param name="settings">The settings to use.</param>
		/// <returns>An <see cref="IdentifierContentGroup"/> representing a colored tooltip, or <c>null</c>.</returns>
		public IdentifierContentGroup? GetIdentifierContentGroup(IHighlighter highlighter, IContextBoundSettingsStore settings) {
			if (!highlighter.IsValid)
				return null;

			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled)) {
				if (TryPresentNonColorized(highlighter, null, settings) is { } content)
					return new IdentifierContentGroup { Identifiers = { content } };
				return null;
			}

			return GetIdentifierContentGroupCore(new DocumentRange(highlighter.Document, highlighter.Range), settings, highlighter);
		}

		/// <summary>
		/// Returns a colored <see cref="IdentifierContentGroup"/> for an identifier at a given <see cref="DocumentRange"/>.
		/// </summary>
		/// <param name="documentRange">The document range where to find a <see cref="IDeclaredElement"/>.</param>
		/// <param name="settings">The settings to use.</param>
		/// <returns>An <see cref="IdentifierContentGroup"/> representing a colored tooltip, or <c>null</c>.</returns>
		public IdentifierContentGroup? GetIdentifierContentGroup(DocumentRange documentRange, IContextBoundSettingsStore settings) {
			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled))
				return null;

			return GetIdentifierContentGroupCore(documentRange, settings, null);
		}

		private IdentifierContentGroup? GetIdentifierContentGroupCore(
			DocumentRange documentRange,
			IContextBoundSettingsStore settings,
			IHighlighter? highlighter) {

			PresentableInfo presentable = FindPresentable(documentRange);
			if (!presentable.IsValid())
				return null;

			IdentifierTooltipContent? standardContent =
				TryPresentColorized(presentable.DeclaredElementInfo, settings)
				?? TryPresentColorized(presentable.PresentableNode, settings)
				?? TryPresentNonColorized(highlighter, presentable.DeclaredElementInfo?.DeclaredElement, settings);

			IdentifierTooltipContent? additionalContent = TryGetAdditionalIdentifierContent(presentable.DeclaredElementInfo, settings, out bool replacesStandardContent);
			if (replacesStandardContent) {
				standardContent = additionalContent;
				additionalContent = null;
			}

			var result = new IdentifierContentGroup();

			if (standardContent is not null)
				result.Identifiers.Add(standardContent);
			if (additionalContent is not null)
				result.Identifiers.Add(additionalContent);

			ITreeNode? node = presentable.DeclaredElementInfo?.TreeNode ?? presentable.PresentableNode.Node;
			result.ArgumentRole = TryGetArgumentRoleContent(node, settings);

			return result;
		}

		private IdentifierTooltipContent? TryGetAdditionalIdentifierContent(
			DeclaredElementInfo? info,
			IContextBoundSettingsStore settings,
			out bool replacesStandardContent) {

			replacesStandardContent = false;

			if (info?.DeclaredElement is not IConstructor constructor || constructor.GetContainingType() is not { } typeElement)
				return null;

			ConstructorReferenceDisplay display = settings.GetValue(GetConstructorSettingsKey(typeElement.IsAttribute()));
			switch (display) {

				case ConstructorReferenceDisplay.TypeOnly:
					replacesStandardContent = true;
					return TryGetTypeIdentifierContentFromConstructor(constructor, info, settings);

				case ConstructorReferenceDisplay.Both:
					return TryGetTypeIdentifierContentFromConstructor(constructor, info, settings);

				default:
					return null;

			}
		}

		private static Expression<Func<IdentifierTooltipSettings, ConstructorReferenceDisplay>> GetConstructorSettingsKey(bool isAttribute) {
			if (isAttribute)
				return s => s.AttributeConstructorReferenceDisplay;
			return s => s.ConstructorReferenceDisplay;
		}

		private IdentifierTooltipContent? TryGetTypeIdentifierContentFromConstructor(
			IConstructor constructor, DeclaredElementInfo constructorInfo, IContextBoundSettingsStore settings) {

			if (constructor.GetContainingType() is not { } typeElement)
				return null;

			var typeInfo = new DeclaredElementInfo(typeElement, constructorInfo.Substitution, constructorInfo.TreeNode, constructorInfo.SourceRange, null);
			return TryPresentColorized(typeInfo, settings);
		}

		private IdentifierTooltipContent? TryPresentColorized(DeclaredElementInfo? info, IContextBoundSettingsStore settings) {
			if (info is null)
				return null;

			PsiLanguageType languageType = info.TreeNode.Language;
			IDeclaredElement element = info.DeclaredElement;
			IPsiModule psiModule = info.TreeNode.GetPsiModule();
			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);
			var reflectionCppTooltipContentProvider = _solution.TryGetComponent<ReflectionCppTooltipContentProvider>();

			RichText? identifierText;
			if (reflectionCppTooltipContentProvider is not null && reflectionCppTooltipContentProvider.IsCppDeclaredElement(element))
				identifierText = reflectionCppTooltipContentProvider.TryPresentCppDeclaredElement(element);
			else {
				identifierText = _colorizerPresenter.TryPresent(
					new DeclaredElementInstance(element, info.Substitution),
					PresenterOptions.ForIdentifierToolTip(settings, !element.IsEnumMember()),
					languageType,
					highlighterIdProvider,
					info.TreeNode,
					out _);
			}

			if (identifierText is null || identifierText.IsEmpty)
				return null;

			var identifierContent = new IdentifierTooltipContent(identifierText, info.SourceRange);

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				identifierContent.Icon = TryGetIcon(element);

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowDocumentation)) {

				XmlNode? xmlDoc = element.GetXMLDoc(true);
				identifierContent.Description = TryGetDescription(element, xmlDoc, psiModule, languageType);

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowObsolete))
					identifierContent.Obsolete = TryRemoveObsoletePrefix(TryGetObsolete(element, psiModule, languageType));

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowReturn))
					identifierContent.Return = TryPresentDocNode(xmlDoc, "returns", languageType, psiModule);

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowValue))
					identifierContent.Value = TryPresentDocNode(xmlDoc, "value", languageType, psiModule);

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowRemarks))
					identifierContent.Remarks = TryPresentDocNode(xmlDoc, "remarks", languageType, psiModule);

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowExceptions))
					identifierContent.Exceptions.AddRange(GetExceptions(xmlDoc, languageType, psiModule));

			}

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowOverloadCount))
				identifierContent.OverloadCount = TryGetOverloadCount(element as IFunction, info.Reference, languageType);

			if (info.DeclaredElement is ITypeElement typeElement) {

				var baseTypeDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.BaseTypeDisplayKind);
				var implementedInterfacesDisplayKind = settings.GetValue((IdentifierTooltipSettings s) => s.ImplementedInterfacesDisplayKind);
				if (baseTypeDisplayKind != BaseTypeDisplayKind.Never
				|| implementedInterfacesDisplayKind != ImplementedInterfacesDisplayKind.Never)
					AddSuperTypes(identifierContent, typeElement, baseTypeDisplayKind, implementedInterfacesDisplayKind, languageType, info.TreeNode, highlighterIdProvider, settings);

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowAttributesUsage) && typeElement.IsAttribute())
					identifierContent.AttributeUsage = GetAttributeUsage((IClass) info.DeclaredElement);

			}

			return identifierContent;
		}

		private static int? TryGetOverloadCount(IFunction? function, IReference? reference, PsiLanguageType languageType) {
			if (function is null || reference is null || function is PredefinedOperator)
				return null;

			var candidateCountProvider = LanguageManager.Instance.TryGetService<IInvocationCandidateCountProvider>(languageType);
			int? candidateCount = candidateCountProvider?.TryGetInvocationCandidateCount(reference);
			if (candidateCount is null || candidateCount.Value <= 0)
				return null;

			return candidateCount.Value - 1;
		}

		private void AddSuperTypes(
			IdentifierTooltipContent identifierContent,
			ITypeElement typeElement,
			BaseTypeDisplayKind baseTypeDisplayKind,
			ImplementedInterfacesDisplayKind implementedInterfacesDisplayKind,
			PsiLanguageType languageType,
			ITreeNode contextualNode,
			HighlighterIdProvider highlighterIdProvider,
			IContextBoundSettingsStore settings) {

			GetSuperTypes(
				typeElement,
				baseTypeDisplayKind,
				implementedInterfacesDisplayKind,
				out DeclaredElementInstance? baseType,
				out IList<DeclaredElementInstance> implementedInterfaces);

			if (baseType is null && implementedInterfaces.Count == 0)
				return;

			PresenterOptions presenterOptions = PresenterOptions.ForBaseTypeOrImplementedInterfaceTooltip(settings);

			if (baseType is not null)
				identifierContent.BaseType = _colorizerPresenter.TryPresent(baseType, presenterOptions, languageType, highlighterIdProvider, contextualNode, out _);

			if (implementedInterfaces.Count > 0) {
				var sortedPresentedInterfaces = new SortedDictionary<string, RichText>(StringComparer.Ordinal);
				foreach (DeclaredElementInstance implementedInterface in implementedInterfaces) {
					if (_colorizerPresenter.TryPresent(implementedInterface, presenterOptions, languageType, highlighterIdProvider, contextualNode, out _) is { } richText)
						sortedPresentedInterfaces[richText.ToString(false)] = richText;
				}
				foreach (RichText richText in sortedPresentedInterfaces.Values)
					identifierContent.ImplementedInterfaces.Add(richText);
			}

		}

		private static void GetSuperTypes(
			ITypeElement typeElement,
			BaseTypeDisplayKind baseTypeDisplayKind,
			ImplementedInterfacesDisplayKind implementedInterfacesDisplayKind,
			out DeclaredElementInstance? baseType,
			out IList<DeclaredElementInstance> implementedInterfaces) {

			baseType = null;
			implementedInterfaces = EmptyList<DeclaredElementInstance>.InstanceList;

			var searchForBaseType = baseTypeDisplayKind != BaseTypeDisplayKind.Never && typeElement is IClass or ITypeParameter;
			bool searchForImplementedInterfaces = implementedInterfacesDisplayKind != ImplementedInterfacesDisplayKind.Never;
			if (!searchForBaseType && !searchForImplementedInterfaces)
				return;

			var foundInterfaces = new LocalList<DeclaredElementInstance>();

			foreach (var superType in typeElement.GetAllSuperTypes()) {
				ITypeElement? superTypeElement = superType.GetTypeElement();

				if (superTypeElement is IClass or IDelegate) {
					if (searchForBaseType) {
						searchForBaseType = false;
						if (MatchesBaseTypeDisplayKind(superTypeElement, baseTypeDisplayKind))
							baseType = new DeclaredElementInstance(superTypeElement, superType.GetSubstitution());
						if (!searchForImplementedInterfaces)
							return;
					}
					continue;
				}

				if (searchForImplementedInterfaces
				&& superTypeElement is IInterface @interface
				&& MatchesImplementedInterfacesDisplayKind(@interface, implementedInterfacesDisplayKind))
					foundInterfaces.Add(new DeclaredElementInstance(superTypeElement, superType.GetSubstitution()));
			}

			implementedInterfaces = foundInterfaces.ResultingList();
		}

		private static bool MatchesBaseTypeDisplayKind(ITypeElement typeElement, BaseTypeDisplayKind displayKind)
			=> displayKind switch {
				BaseTypeDisplayKind.Never => false,
				BaseTypeDisplayKind.SolutionCode => typeElement is not ICompiledElement,
				BaseTypeDisplayKind.SolutionCodeAndNonSystemExternalCode => !(typeElement is ICompiledElement && typeElement.IsInSystemLikeNamespace()),
				BaseTypeDisplayKind.OnlyIfNotSystemObject => !typeElement.IsObjectClass(),
				BaseTypeDisplayKind.Always => true,
				_ => false
			};

		private static bool MatchesImplementedInterfacesDisplayKind(ITypeElement typeElement, ImplementedInterfacesDisplayKind displayKind)
			=> displayKind switch {
				ImplementedInterfacesDisplayKind.Never => false,
				ImplementedInterfacesDisplayKind.SolutionCode => typeElement is not ICompiledElement,
				ImplementedInterfacesDisplayKind.SolutionCodeAndNonSystemExternalCode => !(typeElement is ICompiledElement && typeElement.IsInSystemLikeNamespace()),
				ImplementedInterfacesDisplayKind.Always => true,
				_ => false
			};

		private static AttributeUsageContent GetAttributeUsage(IClass attributeClass) {
			AttributeTargets targets;
			bool allowMultiple;
			bool inherited;

			if (attributeClass.GetAttributeInstances(PredefinedType.ATTRIBUTE_USAGE_ATTRIBUTE_CLASS, true).FirstOrDefault() is { } attributeUsage) {
				targets = (AttributeTargets?) (attributeUsage.PositionParameter(0).ConstantValue.Value as int?) ?? AttributeTargets.All;
				allowMultiple = attributeUsage.NamedParameter(nameof(AttributeUsageAttribute.AllowMultiple)).ConstantValue.Value as bool? ?? false;
				inherited = attributeUsage.NamedParameter(nameof(AttributeUsageAttribute.Inherited)).ConstantValue.Value as bool? ?? true;
			}
			else {
				targets = AttributeTargets.All;
				allowMultiple = attributeClass.HasAttributeInstance(PredefinedType.WINRT_ALLOW_MULTIPLE_ATTRIBUTE_CLASS, true);
				inherited = true;
			}

			return new AttributeUsageContent(targets.ToHumanReadableString(), allowMultiple, inherited);
		}

		private IdentifierTooltipContent? TryPresentNonColorized(
			IHighlighter? highlighter,
			IDeclaredElement? element,
			IContextBoundSettingsStore settings) {

			if (highlighter?.TryGetTooltip(HighlighterTooltipKind.TextEditor) is not { } richTextToolTip)
				return null;

			RichText richText = richTextToolTip.RichText;
			if (richText.IsNullOrEmpty())
				return null;

			var identifierContent = new IdentifierTooltipContent(richText, highlighter.Range);
			if (element is not null && settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				identifierContent.Icon = TryGetIcon(element);
			return identifierContent;
		}

		private static RichText? TryPresentDocNode(
			XmlNode? xmlDoc,
			string nodeName,
			PsiLanguageType languageType,
			IPsiModule psiModule) {

			XmlNode? returnsNode = xmlDoc?.SelectSingleNode(nodeName);
			if (returnsNode is null || !returnsNode.HasChildNodes)
				return null;

			var richText = XmlDocRichTextPresenter.Run(returnsNode, false, languageType, DeclaredElementPresenterTextStyles.Empty, psiModule).RichText;
			return richText.IsNullOrEmpty() ? null : richText;
		}

		private static IEnumerable<ExceptionContent> GetExceptions(
			XmlNode? xmlDoc,
			PsiLanguageType languageType,
			IPsiModule psiModule) {

			if (xmlDoc?.SelectNodes("exception") is not { Count: > 0 } exceptionNodes)
				return EmptyList<ExceptionContent>.InstanceList;

			var exceptions = new LocalList<ExceptionContent>();
			foreach (XmlNode exceptionNode in exceptionNodes) {
				if (TryExtractException(exceptionNode as XmlElement, languageType, psiModule) is { } exceptionContent)
					exceptions.Add(exceptionContent);
			}
			return exceptions.ResultingList();
		}

		private static ExceptionContent? TryExtractException(
			XmlElement? exceptionElement,
			PsiLanguageType languageType,
			IPsiModule psiModule) {

			string? cref = exceptionElement?.GetAttribute("cref");
			if (String.IsNullOrEmpty(cref))
				return null;

			cref = XmlDocPresenterUtil.ProcessCref(cref);
			if (String.IsNullOrEmpty(cref))
				return null;

			var exceptionContent = new ExceptionContent(cref);
			if (exceptionElement!.HasChildNodes) {
				RichText richText = XmlDocRichTextPresenter.Run(exceptionElement, false, languageType, DeclaredElementPresenterTextStyles.Empty, psiModule).RichText;
				if (!richText.IsNullOrEmpty())
					exceptionContent.Description = richText;
			}
			return exceptionContent;
		}

		/// <summary>
		/// Returns the description of an element, if available.
		/// </summary>
		/// <param name="element">The element whose description will be returned.</param>
		/// <param name="xmlDoc">The XML documentation for <paramref name="element"/>.</param>
		/// <param name="psiModule">The PSI module of the file containing the identifier.</param>
		/// <param name="languageType">The type of language used to present the identifier.</param>
		private RichText? TryGetDescription(
			IDeclaredElement element,
			XmlNode? xmlDoc,
			IPsiModule psiModule,
			PsiLanguageType languageType) {

			if (TryPresentDocNode(xmlDoc, "summary", languageType, psiModule) is { } richText)
				return richText;

			return _declaredElementDescriptionPresenter
				.GetDeclaredElementDescription(element, DeclaredElementDescriptionStyle.NO_OBSOLETE_SUMMARY_STYLE, languageType, psiModule)
				?.RichText;
		}

		/// <summary>
		/// Returns the obsolete message of an element, if available.
		/// </summary>
		/// <param name="element">The element whose description will be returned.</param>
		/// <param name="psiModule">The PSI module of the file containing the identifier.</param>
		/// <param name="languageType">The type of language used to present the identifier.</param>
		private RichText? TryGetObsolete(
			IDeclaredElement element,
			IPsiModule psiModule,
			PsiLanguageType languageType)
			=> _declaredElementDescriptionPresenter
				.GetDeclaredElementDescription(element, DeclaredElementDescriptionStyle.OBSOLETE_DESCRIPTION, languageType, psiModule)
				?.RichText;

		private static RichText? TryRemoveObsoletePrefix(RichText? text) {
			if (text is null)
				return null;

			const string obsoletePrefix = "Obsolete: ";

			IList<RichString> parts = text.GetFormattedParts();
			if (parts.Count >= 2 && parts[0].Text == obsoletePrefix)
				return text.Split(obsoletePrefix.Length)[1];
			return text;
		}

		private IconId? TryGetIcon(IDeclaredElement declaredElement) {
			var psiIconManager = _solution.GetComponent<PsiIconManager>();
			return psiIconManager.GetImage(declaredElement, declaredElement.PresentationLanguage, true);
		}

		private IdentifierTooltipContent? TryPresentColorized(PresentableNode presentableNode, IContextBoundSettingsStore settings) {
			if (presentableNode.Node is not { } node)
				return null;

			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);

			RichText? identifierText = _colorizerPresenter.TryPresent(
				node,
				PresenterOptions.ForIdentifierToolTip(settings, true),
				node.Language,
				highlighterIdProvider);

			if (identifierText is null || identifierText.IsEmpty)
				return null;

			var identifierContent = new IdentifierTooltipContent(identifierText, node.GetDocumentRange().TextRange);

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				identifierContent.Icon = presentableNode.Icon;

			return identifierContent;
		}

		/// <summary>
		/// Finds a valid presentable element represented at a given <see cref="DocumentRange"/>.
		/// </summary>
		/// <param name="documentRange">The document range where to find a <see cref="IDeclaredElement"/> or a <see cref="ILiteralExpression"/>.</param>
		/// <returns>A <see cref="PresentableInfo"/> which may not be valid if nothing was found.</returns>
		private PresentableInfo FindPresentable(DocumentRange documentRange) {
			if (documentRange.Document is not { } document || !documentRange.IsValid())
				return default;

			IPsiServices psiServices = _solution.GetPsiServices();
			if (!psiServices.Files.AllDocumentsAreCommitted || psiServices.Caches.HasDirtyFiles)
				return default;

			return document
				.GetPsiSourceFiles(_solution)
				.SelectMany(
					psiSourceFile => psiServices.Files.GetPsiFiles(psiSourceFile, documentRange),
					(_, file) => FindPresentable(documentRange, file))
				.FirstOrDefault(info => info.IsValid());
		}

		/// <summary>
		/// Finds an element at a given file range, either a reference or a declaration.
		/// </summary>
		/// <param name="range">The range to get the element in <paramref name="file"/>.</param>
		/// <param name="file">The file to search into.</param>
		/// <returns>A <see cref="PresentableInfo"/> at range <paramref name="range"/> in <paramref name="file"/>, which may not be valid if nothing was found.</returns>
		private static PresentableInfo FindPresentable(DocumentRange range, IFile file) {
			if (!file.IsValid())
				return default;

			TreeTextRange treeTextRange = file.Translate(range);
			if (!treeTextRange.IsValid())
				return default;

			var references = file.FindReferencesAt(treeTextRange);
			DeclaredElementInfo? bestReference = null;
			if (references.Count > 0) {
				bestReference = GetBestReference(references.ToArray());
				if (bestReference is not null
				&& bestReference.Reference?.GetTreeNode() is not ICollectionElementInitializer) // we may do better than showing a collection initializer
					return new PresentableInfo(bestReference);
			}

			// FindNodeAt seems to return the previous node on single-char literals (eg '0'). FindNodesAt is fine.
			var node = file.FindNodesAt<ITreeNode>(treeTextRange).FirstOrDefault();
			if (node is not null && node.IsValid()) {

				if ((FindDeclaration(node, file) ?? FindSpecialElement(node, file)) is { } declaredElementInfo)
					return new PresentableInfo(declaredElementInfo);

				PresentableNode presentableNode = FindPresentableNode(node, file);
				if (presentableNode.Node is not null)
					return new PresentableInfo(presentableNode);

			}

			return new PresentableInfo(bestReference);
		}

		/// <summary>
		/// Gets the best reference (the "deepest" one) from a collection of references.
		/// </summary>
		/// <param name="references">A collection of references.</param>
		/// <returns>The <see cref="DeclaredElementInfo"/> corresponding to the best reference.</returns>
		private static DeclaredElementInfo? GetBestReference(IReference[] references) {
			SortReferences(references);

			foreach (IReference reference in references) {
				IResolveResult resolveResult = reference.Resolve().Result;
				if (reference.CheckResolveResult() == ResolveErrorType.DYNAMIC)
					return null;

				if (resolveResult.DeclaredElement is { } foundElement) {
					var referenceRange = reference.GetDocumentRange().TextRange;
					return new DeclaredElementInfo(foundElement, resolveResult.Substitution, reference.GetTreeNode(), referenceRange, reference);
				}
			}

			return null;
		}

		private static void SortReferences(IReference[] references) {
			int count = references.Length;
			if (count <= 1)
				return;

			int[] pathLengths = new int[count];
			for (int i = 0; i < count; ++i) {
				ITreeNode treeNode = references[i].GetTreeNode();
				int pathToRootLength = treeNode.PathToRoot().Count();
				pathLengths[i] = treeNode is ICollectionElementInitializer
					? Int32.MaxValue - pathToRootLength // collection initializers have the lowest priority, and are reversed if nested
					: pathToRootLength;
			}

			Array.Sort(pathLengths, references);
		}

		private static DeclaredElementInfo? FindDeclaration(ITreeNode node, IFile file) {
			if (node.GetContainingNode<IDeclaration>(true) is not { } declaration)
				return null;

			TreeTextRange nameRange = declaration.GetNameRange();
			if (!nameRange.IntersectsOrContacts(node.GetTreeTextRange()))
				return null;

			if (declaration.DeclaredElement is not { } declaredElement)
				return null;

			return new DeclaredElementInfo(declaredElement, EmptySubstitution.INSTANCE, node, file.GetDocumentRange(nameRange).TextRange, null);
		}

		private static DeclaredElementInfo? FindSpecialElement(ITreeNode node, IFile file)
			=> LanguageManager.Instance.TryGetService<IPresentableNodeFinder>(file.Language) is { } finder
				&& finder.FindDeclaredElement(node, file, out TextRange sourceRange) is { } declaredElementInstance
					? new DeclaredElementInfo(declaredElementInstance.Element, declaredElementInstance.Substitution, node, sourceRange, null)
					: null;

		private static PresentableNode FindPresentableNode(ITreeNode node, IFile file) {
			var finder = LanguageManager.Instance.TryGetService<IPresentableNodeFinder>(file.Language);
			if (finder is null)
				return default;

			return finder.FindPresentableNode(node);
		}

		private ArgumentRoleTooltipContent? TryGetArgumentRoleContent(ITreeNode? node, IContextBoundSettingsStore settings) {
			if (node is null || !settings.GetValue((IdentifierTooltipSettings s) => s.ShowArgumentsRole))
				return null;

			var argument = node.GetContainingNode<IArgument>();
			if (argument?.MatchingParameter is not { } parameterInstance)
				return null;

			IParameter parameter = parameterInstance.Element;
			if (parameter.ContainingParametersOwner is not { } parametersOwner)
				return null;

			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);

			RichText final = new RichText("Argument of ", TextStyle.Default);
			var parametersOwnerDisplay = _colorizerPresenter.TryPresent(
				new DeclaredElementInstance(parametersOwner, parameterInstance.Substitution),
				PresenterOptions.ForArgumentRoleParametersOwnerToolTip(settings),
				argument.Language,
				highlighterIdProvider,
				node,
				out _);

			if (parametersOwnerDisplay is not null)
				final.Append(parametersOwnerDisplay);

			final.Append(": ", TextStyle.Default);

			var parameterDisplay = _colorizerPresenter.TryPresent(
				parameterInstance,
				PresenterOptions.ForArgumentRoleParameterToolTip(settings),
				argument.Language,
				highlighterIdProvider,
				node,
				out _);

			if (parameterDisplay is not null)
				final.Append(parameterDisplay);

			var content = new ArgumentRoleTooltipContent(final, argument.GetDocumentRange().TextRange) {
				Description = TryGetDescription(parameter, parameter.GetXMLDoc(true), parameter.Module, argument.Language)
			};

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				content.Icon = PsiSymbolsThemedIcons.Parameter.Id;

			return content;
		}

		public IdentifierTooltipContentProvider(
			ISolution solution,
			ColorizerPresenter colorizerPresenter,
			IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter,
			HighlighterIdProviderFactory highlighterIdProviderFactory) {
			_solution = solution;
			_colorizerPresenter = colorizerPresenter;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}