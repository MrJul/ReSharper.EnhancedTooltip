using System.Linq.Expressions;
using GammaJul.ReSharper.EnhancedTooltip.Psi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using GammaJul.ReSharper.EnhancedTooltip.Settings;
using JetBrains.Annotations;
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

			[NotNull] internal readonly IDeclaredElement DeclaredElement;
			[NotNull] internal readonly ISubstitution Substitution;
			[NotNull] internal readonly ITreeNode TreeNode;
			internal readonly TextRange SourceRange;
			[CanBeNull] internal readonly IReference Reference;

			public DeclaredElementInfo(
				[NotNull] IDeclaredElement declaredElement,
				[NotNull] ISubstitution substitution,
				[NotNull] ITreeNode treeNode,
				TextRange sourceRange,
				[CanBeNull] IReference reference) {
				DeclaredElement = declaredElement;
				Substitution = substitution;
				TreeNode = treeNode;
				SourceRange = sourceRange;
				Reference = reference;
			}

		}

		private readonly struct PresentableInfo {

			[CanBeNull] internal readonly DeclaredElementInfo DeclaredElementInfo;
			internal readonly PresentableNode PresentableNode;

			public bool IsValid()
				=> DeclaredElementInfo != null && DeclaredElementInfo.DeclaredElement.IsValid()
				|| PresentableNode.Node != null;

			public PresentableInfo([CanBeNull] DeclaredElementInfo declaredElementInfo) {
				DeclaredElementInfo = declaredElementInfo;
				PresentableNode = default;
			}

			public PresentableInfo(PresentableNode presentableNode) {
				DeclaredElementInfo = null;
				PresentableNode = presentableNode;
			}

		}

		[NotNull] private readonly ISolution _solution;
		[NotNull] private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		[NotNull] private readonly HighlighterIdProviderFactory _highlighterIdProviderFactory;
		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		
		/// <summary>
		/// Returns a colored <see cref="IdentifierContentGroup"/> for an identifier represented by a <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter representing the identifier.</param>
		/// <param name="settings">The settings to use.</param>
		/// <returns>An <see cref="IdentifierContentGroup"/> representing a colored tooltip, or <c>null</c>.</returns>
		[CanBeNull]
		public IdentifierContentGroup GetIdentifierContentGroup([NotNull] IHighlighter highlighter, [NotNull] IContextBoundSettingsStore settings) {
			if (!highlighter.IsValid)
				return null;

			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled)) {
				IdentifierTooltipContent content = TryPresentNonColorized(highlighter, null, settings);
				if (content != null)
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
		[CanBeNull]
		public IdentifierContentGroup GetIdentifierContentGroup(DocumentRange documentRange, [NotNull] IContextBoundSettingsStore settings) {
			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled))
				return null;

			return GetIdentifierContentGroupCore(documentRange, settings, null);
		}

		[CanBeNull]
		private IdentifierContentGroup GetIdentifierContentGroupCore(
			DocumentRange documentRange,
			[NotNull] IContextBoundSettingsStore settings,
			[CanBeNull] IHighlighter highlighter) {

			PresentableInfo presentable = FindPresentable(documentRange);
			if (!presentable.IsValid())
				return null;

			IdentifierTooltipContent standardContent =
				TryPresentColorized(presentable.DeclaredElementInfo, settings)
				?? TryPresentColorized(presentable.PresentableNode, settings)
				?? TryPresentNonColorized(highlighter, presentable.DeclaredElementInfo?.DeclaredElement, settings);

			IdentifierTooltipContent additionalContent = TryGetAdditionalIdentifierContent(presentable.DeclaredElementInfo, settings, out bool replacesStandardContent);
			if (replacesStandardContent) {
				standardContent = additionalContent;
				additionalContent = null;
			}

			var result = new IdentifierContentGroup();

			if (standardContent != null)
				result.Identifiers.Add(standardContent);
			if (additionalContent != null)
				result.Identifiers.Add(additionalContent);

			ITreeNode node = presentable.DeclaredElementInfo?.TreeNode ?? presentable.PresentableNode.Node;
			result.ArgumentRole = TryGetArgumentRoleContent(node, settings);

			return result;
		}

		[CanBeNull]
		private IdentifierTooltipContent TryGetAdditionalIdentifierContent(
			[CanBeNull] DeclaredElementInfo info,
			[NotNull] IContextBoundSettingsStore settings,
			out bool replacesStandardContent) {
			
			replacesStandardContent = false;

			if (info == null)
				return null;

			var constructor = info.DeclaredElement as IConstructor;
			ITypeElement typeElement = constructor?.GetContainingType();
			if (typeElement == null)
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

		[NotNull]
		private static Expression<Func<IdentifierTooltipSettings, ConstructorReferenceDisplay>> GetConstructorSettingsKey(bool isAttribute) {
			if (isAttribute)
				return s => s.AttributeConstructorReferenceDisplay;
			return s => s.ConstructorReferenceDisplay;
		}

		[CanBeNull]
		private IdentifierTooltipContent TryGetTypeIdentifierContentFromConstructor(
			[NotNull] IConstructor constructor, [NotNull] DeclaredElementInfo constructorInfo, [NotNull] IContextBoundSettingsStore settings) {

			ITypeElement typeElement = constructor.GetContainingType();
			if (typeElement == null)
				return null;

			var typeInfo = new DeclaredElementInfo(typeElement, constructorInfo.Substitution, constructorInfo.TreeNode, constructorInfo.SourceRange, null);
			return TryPresentColorized(typeInfo, settings);
		}

		[CanBeNull]
		private IdentifierTooltipContent TryPresentColorized([CanBeNull] DeclaredElementInfo info, [NotNull] IContextBoundSettingsStore settings) {
			if (info == null)
				return null;

			PsiLanguageType languageType = info.TreeNode.Language;
			IDeclaredElement element = info.DeclaredElement;
			IPsiModule psiModule = info.TreeNode.GetPsiModule();
			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);
			var reflectionCppTooltipContentProvider = _solution.TryGetComponent<ReflectionCppTooltipContentProvider>();

			RichText identifierText;
			if (reflectionCppTooltipContentProvider != null && reflectionCppTooltipContentProvider.IsCppDeclaredElement(element))
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

			if (identifierText == null || identifierText.IsEmpty)
				return null;

			var identifierContent = new IdentifierTooltipContent(identifierText, info.SourceRange);
			
			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				identifierContent.Icon = TryGetIcon(element);

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowDocumentation)) {

				XmlNode xmlDoc = element.GetXMLDoc(true);
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

		private static int? TryGetOverloadCount([CanBeNull] IFunction function, [CanBeNull] IReference reference, PsiLanguageType languageType) {
			if (function == null || reference == null || function is PredefinedOperator)
				return null;

			var candidateCountProvider = LanguageManager.Instance.TryGetService<IInvocationCandidateCountProvider>(languageType);
			int? candidateCount = candidateCountProvider?.TryGetInvocationCandidateCount(reference);
			if (candidateCount == null || candidateCount.Value <= 0)
				return null;

			return candidateCount.Value - 1;
		}

		private void AddSuperTypes(
			[NotNull] IdentifierTooltipContent identifierContent,
			[NotNull] ITypeElement typeElement,
			BaseTypeDisplayKind baseTypeDisplayKind,
			ImplementedInterfacesDisplayKind implementedInterfacesDisplayKind,
			[NotNull] PsiLanguageType languageType,
			[NotNull] ITreeNode contextualNode,
			[NotNull] HighlighterIdProvider highlighterIdProvider,
			[NotNull] IContextBoundSettingsStore settings) {

			GetSuperTypes(
				typeElement,
				baseTypeDisplayKind,
				implementedInterfacesDisplayKind,
				out DeclaredElementInstance baseType,
				out IList<DeclaredElementInstance> implementedInterfaces);

			if (baseType == null && implementedInterfaces.Count == 0)
				return;

			PresenterOptions presenterOptions = PresenterOptions.ForBaseTypeOrImplementedInterfaceTooltip(settings);

			if (baseType != null)
				identifierContent.BaseType = _colorizerPresenter.TryPresent(baseType, presenterOptions, languageType, highlighterIdProvider, contextualNode, out _);

			if (implementedInterfaces.Count > 0) {
				var sortedPresentedInterfaces = new SortedDictionary<string, RichText>(StringComparer.Ordinal);
				foreach (DeclaredElementInstance implementedInterface in implementedInterfaces) {
					RichText richText = _colorizerPresenter.TryPresent(implementedInterface, presenterOptions, languageType, highlighterIdProvider, contextualNode, out _);
					if (richText != null)
						sortedPresentedInterfaces[richText.ToString(false)] = richText;
				}
				foreach (RichText richText in sortedPresentedInterfaces.Values)
					identifierContent.ImplementedInterfaces.Add(richText);
			}

		}

		private static void GetSuperTypes(
			[NotNull] ITypeElement typeElement,
			BaseTypeDisplayKind baseTypeDisplayKind,
			ImplementedInterfacesDisplayKind implementedInterfacesDisplayKind,
			[CanBeNull] out DeclaredElementInstance baseType,
			[NotNull] out IList<DeclaredElementInstance> implementedInterfaces) {

			baseType = null;
			implementedInterfaces = EmptyList<DeclaredElementInstance>.InstanceList;

			var searchForBaseType = baseTypeDisplayKind != BaseTypeDisplayKind.Never && (typeElement is IClass || typeElement is ITypeParameter);
			bool searchForImplementedInterfaces = implementedInterfacesDisplayKind != ImplementedInterfacesDisplayKind.Never;
			if (!searchForBaseType && !searchForImplementedInterfaces)
				return;

			var foundInterfaces = new LocalList<DeclaredElementInstance>();

			foreach (var superType in typeElement.GetAllSuperTypes()) {
				ITypeElement superTypeElement = superType.GetTypeElement();

				if (superTypeElement is IClass || superTypeElement is IDelegate) {
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

		private static bool MatchesBaseTypeDisplayKind([NotNull] ITypeElement typeElement, BaseTypeDisplayKind displayKind) {
			switch (displayKind) {
				case BaseTypeDisplayKind.Never:
					return false;
				case BaseTypeDisplayKind.SolutionCode:
					return !(typeElement is ICompiledElement);
				case BaseTypeDisplayKind.SolutionCodeAndNonSystemExternalCode:
					return !(typeElement is ICompiledElement && typeElement.IsInSystemLikeNamespace());
				case BaseTypeDisplayKind.OnlyIfNotSystemObject:
					return !typeElement.IsObjectClass();
				case BaseTypeDisplayKind.Always:
					return true;
				default:
					return false;
			}
		}

		private static bool MatchesImplementedInterfacesDisplayKind([NotNull] ITypeElement typeElement, ImplementedInterfacesDisplayKind displayKind) {
			switch (displayKind) {
				case ImplementedInterfacesDisplayKind.Never:
					return false;
				case ImplementedInterfacesDisplayKind.SolutionCode:
					return !(typeElement is ICompiledElement);
				case ImplementedInterfacesDisplayKind.SolutionCodeAndNonSystemExternalCode:
					return !(typeElement is ICompiledElement && typeElement.IsInSystemLikeNamespace());
				case ImplementedInterfacesDisplayKind.Always:
					return true;
				default:
					return false;
			}
		}

		[NotNull]
		private static AttributeUsageContent GetAttributeUsage([NotNull] IClass attributeClass) {
			AttributeTargets targets;
			bool allowMultiple;
			bool inherited;

			var attributeUsage = attributeClass.GetAttributeInstances(PredefinedType.ATTRIBUTE_USAGE_ATTRIBUTE_CLASS, true).FirstOrDefault();
			if (attributeUsage == null) {
				targets = AttributeTargets.All;
				allowMultiple = attributeClass.HasAttributeInstance(PredefinedType.WINRT_ALLOW_MULTIPLE_ATTRIBUTE_CLASS, true);
				inherited = true;
			}
			else {
				targets = ((AttributeTargets?) (attributeUsage.PositionParameter(0).ConstantValue.Value as int?)) ?? AttributeTargets.All;
				allowMultiple = attributeUsage.NamedParameter(nameof(AttributeUsageAttribute.AllowMultiple)).ConstantValue.Value as bool? ?? false;
				inherited = attributeUsage.NamedParameter(nameof(AttributeUsageAttribute.Inherited)).ConstantValue.Value as bool? ?? true;
			}

			return new AttributeUsageContent(targets.ToHumanReadableString(), allowMultiple, inherited);
		}

		[CanBeNull]
		private IdentifierTooltipContent TryPresentNonColorized(
			[CanBeNull] IHighlighter highlighter,
			[CanBeNull] IDeclaredElement element,
			[NotNull] IContextBoundSettingsStore settings) {

			RichTextBlock richTextToolTip = highlighter?.TryGetTooltip(HighlighterTooltipKind.TextEditor);
			if (richTextToolTip == null)
				return null;

			RichText richText = richTextToolTip.RichText;
			if (richText.IsNullOrEmpty())
				return null;

			var identifierContent = new IdentifierTooltipContent(richText, highlighter.Range);
			if (element != null && settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				identifierContent.Icon = TryGetIcon(element);
			return identifierContent;
		}

		[CanBeNull]
		private static RichText TryPresentDocNode(
			[CanBeNull] XmlNode xmlDoc,
			[NotNull] string nodeName,
			[NotNull] PsiLanguageType languageType,
			[NotNull] IPsiModule psiModule) {

			XmlNode returnsNode = xmlDoc?.SelectSingleNode(nodeName);
			if (returnsNode == null || !returnsNode.HasChildNodes)
				return null;

			var richText = XmlDocRichTextPresenter.Run(returnsNode, false, languageType, DeclaredElementPresenterTextStyles.Empty, psiModule).RichText;
			return richText.IsNullOrEmpty() ? null : richText;
		}

		[NotNull]
		private static IEnumerable<ExceptionContent> GetExceptions(
			[CanBeNull] XmlNode xmlDoc,
			[NotNull] PsiLanguageType languageType,
			[NotNull] IPsiModule psiModule) {

			XmlNodeList exceptionNodes = xmlDoc?.SelectNodes("exception");
			if (exceptionNodes == null || exceptionNodes.Count == 0)
				return EmptyList<ExceptionContent>.InstanceList;

			var exceptions = new LocalList<ExceptionContent>();
			foreach (XmlNode exceptionNode in exceptionNodes) {
				ExceptionContent exceptionContent = TryExtractException(exceptionNode as XmlElement, languageType, psiModule);
				if (exceptionContent != null)
					exceptions.Add(exceptionContent);
			}
			return exceptions.ResultingList();
		}

		[CanBeNull]
		private static ExceptionContent TryExtractException(
			[CanBeNull] XmlElement exceptionElement,
			[NotNull] PsiLanguageType languageType,
			[NotNull] IPsiModule psiModule) {

			string cref = exceptionElement?.GetAttribute("cref");
			if (String.IsNullOrEmpty(cref))
				return null;

			cref = XmlDocPresenterUtil.ProcessCref(cref);
			if (String.IsNullOrEmpty(cref))
				return null;

			var exceptionContent = new ExceptionContent(cref);
			if (exceptionElement.HasChildNodes) {
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
		[CanBeNull]
		private RichText TryGetDescription(
			[NotNull] IDeclaredElement element,
			[CanBeNull] XmlNode xmlDoc,
			[NotNull] IPsiModule psiModule,
			[NotNull] PsiLanguageType languageType) {

			RichText richText = TryPresentDocNode(xmlDoc, "summary", languageType, psiModule);
			if (richText != null)
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
		[CanBeNull]
		private RichText TryGetObsolete(
			[NotNull] IDeclaredElement element,
			[NotNull] IPsiModule psiModule,
			[NotNull] PsiLanguageType languageType)
			=> _declaredElementDescriptionPresenter
				.GetDeclaredElementDescription(element, DeclaredElementDescriptionStyle.OBSOLETE_DESCRIPTION, languageType, psiModule)
				?.RichText;

		[CanBeNull]
		private static RichText TryRemoveObsoletePrefix([CanBeNull] RichText text) {
			if (text == null)
				return null;

			const string obsoletePrefix = "Obsolete: ";

			IList<RichString> parts = text.GetFormattedParts();
			if (parts.Count >= 2 && parts[0].Text == obsoletePrefix)
				return text.Split(obsoletePrefix.Length)[1];
			return text;
		}

		[CanBeNull]
		private IconId TryGetIcon([NotNull] IDeclaredElement declaredElement) {
			var psiIconManager = _solution.GetComponent<PsiIconManager>();
			return psiIconManager.GetImage(declaredElement, declaredElement.PresentationLanguage, true);
		}

		[CanBeNull]
		private IdentifierTooltipContent TryPresentColorized(PresentableNode presentableNode, [NotNull] IContextBoundSettingsStore settings) {
			ITreeNode node = presentableNode.Node;
			if (node == null)
				return null;

			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);

			RichText identifierText = _colorizerPresenter.TryPresent(
				node,
				PresenterOptions.ForIdentifierToolTip(settings, true),
				node.Language,
				highlighterIdProvider);

			if (identifierText == null || identifierText.IsEmpty)
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
			IDocument document = documentRange.Document;
			if (document == null || !documentRange.IsValid())
				return default;
			
			IPsiServices psiServices = _solution.GetPsiServices();
			if (!psiServices.Files.AllDocumentsAreCommitted || psiServices.Caches.HasDirtyFiles)
				return default;

			return document
				.GetPsiSourceFiles(_solution)
				.SelectMany(
					psiSourceFile => psiServices.Files.GetPsiFiles(psiSourceFile, documentRange),
					(psiSourceFile, file) => FindPresentable(documentRange, file))
				.FirstOrDefault(info => info.IsValid());
		}

		/// <summary>
		/// Finds an element at a given file range, either a reference or a declaration.
		/// </summary>
		/// <param name="range">The range to get the element in <paramref name="file"/>.</param>
		/// <param name="file">The file to search into.</param>
		/// <returns>A <see cref="PresentableInfo"/> at range <paramref name="range"/> in <paramref name="file"/>, which may not be valid if nothing was found.</returns>
		private static PresentableInfo FindPresentable(DocumentRange range, [NotNull] IFile file) {
			if (!file.IsValid())
				return default;

			TreeTextRange treeTextRange = file.Translate(range);
			if (!treeTextRange.IsValid())
				return default;

			var references = file.FindReferencesAt(treeTextRange);
			DeclaredElementInfo bestReference = null;
			if (references.Count > 0) {
				bestReference = GetBestReference(references.ToArray());
				if (bestReference != null
				&& !(bestReference.Reference?.GetTreeNode() is ICollectionElementInitializer)) // we may do better than showing a collection initializer
					return new PresentableInfo(bestReference);
			}

			// FindNodeAt seems to return the previous node on single-char literals (eg '0'). FindNodesAt is fine.
			var node = file.FindNodesAt<ITreeNode>(treeTextRange).FirstOrDefault();
			if (node != null && node.IsValid()) {
				
				DeclaredElementInfo declaredElementInfo = FindDeclaration(node, file) ?? FindSpecialElement(node, file);
				if (declaredElementInfo != null)
					return new PresentableInfo(declaredElementInfo);

				PresentableNode presentableNode = FindPresentableNode(node, file);
				if (presentableNode.Node != null)
					return new PresentableInfo(presentableNode);

			}

			return new PresentableInfo(bestReference);
		}

		/// <summary>
		/// Gets the best reference (the "deepest" one) from a collection of references.
		/// </summary>
		/// <param name="references">A collection of references.</param>
		/// <returns>The <see cref="DeclaredElementInfo"/> corresponding to the best reference.</returns>
		[CanBeNull]
		private static DeclaredElementInfo GetBestReference([NotNull] IReference[] references) {
			SortReferences(references);

			foreach (IReference reference in references) {
				IResolveResult resolveResult = reference.Resolve().Result;
				if (reference.CheckResolveResult() == ResolveErrorType.DYNAMIC)
					return null;

				IDeclaredElement foundElement = resolveResult.DeclaredElement;
				if (foundElement != null) {
					var referenceRange = reference.GetDocumentRange().TextRange;
					return new DeclaredElementInfo(foundElement, resolveResult.Substitution, reference.GetTreeNode(), referenceRange, reference);
				}
			}

			return null;
		}
		
		private static void SortReferences([NotNull] IReference[] references) {
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

		[CanBeNull]
		private static DeclaredElementInfo FindDeclaration([NotNull] ITreeNode node, [NotNull] IFile file) {
			var declaration = node.GetContainingNode<IDeclaration>(true);
			if (declaration == null)
				return null;

			TreeTextRange nameRange = declaration.GetNameRange();
			if (!nameRange.IntersectsOrContacts(node.GetTreeTextRange()))
				return null;

			IDeclaredElement declaredElement = declaration.DeclaredElement;
			if (declaredElement == null)
				return null;

			return new DeclaredElementInfo(declaredElement, EmptySubstitution.INSTANCE, node, file.GetDocumentRange(nameRange).TextRange, null);
		}

		[CanBeNull]
		private static DeclaredElementInfo FindSpecialElement([NotNull] ITreeNode node, [NotNull] IFile file) {
			var finder = LanguageManager.Instance.TryGetService<IPresentableNodeFinder>(file.Language);
			if (finder == null)
				return null;

			DeclaredElementInstance declaredElementInstance = finder.FindDeclaredElement(node, file, out TextRange sourceRange);
			if (declaredElementInstance == null)
				return null;

			return new DeclaredElementInfo(declaredElementInstance.Element, declaredElementInstance.Substitution, node, sourceRange, null);
		}

		private static PresentableNode FindPresentableNode([NotNull] ITreeNode node, [NotNull] IFile file) {
			var finder = LanguageManager.Instance.TryGetService<IPresentableNodeFinder>(file.Language);
			if (finder == null)
				return default;

			return finder.FindPresentableNode(node);
		}

		[CanBeNull]
		private ArgumentRoleTooltipContent TryGetArgumentRoleContent([CanBeNull] ITreeNode node, [NotNull] IContextBoundSettingsStore settings) {
			if (node == null || !settings.GetValue((IdentifierTooltipSettings s) => s.ShowArgumentsRole))
				return null;

			var argument = node.GetContainingNode<IArgument>();
			DeclaredElementInstance<IParameter> parameterInstance = argument?.MatchingParameter;
			if (parameterInstance == null)
				return null;

			IParameter parameter = parameterInstance.Element;
			IParametersOwner parametersOwner = parameter.ContainingParametersOwner;
			if (parametersOwner == null)
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

			if (parametersOwnerDisplay != null)
				final.Append(parametersOwnerDisplay);

			final.Append(": ", TextStyle.Default);

			var parameterDisplay = _colorizerPresenter.TryPresent(
				parameterInstance,
				PresenterOptions.ForArgumentRoleParameterToolTip(settings),
				argument.Language,
				highlighterIdProvider,
				node,
				out _);

			if (parameterDisplay != null)
				final.Append(parameterDisplay);

			var content = new ArgumentRoleTooltipContent(final, argument.GetDocumentRange().TextRange) {
				Description = TryGetDescription(parameter, parameter.GetXMLDoc(true), parameter.Module, argument.Language)
			};

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				content.Icon = PsiSymbolsThemedIcons.Parameter.Id;

			return content;
		}

		public IdentifierTooltipContentProvider(
			[NotNull] ISolution solution,
			[NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter,
			[NotNull] HighlighterIdProviderFactory highlighterIdProviderFactory) {
			_solution = solution;
			_colorizerPresenter = colorizerPresenter;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_highlighterIdProviderFactory = highlighterIdProviderFactory;
		}

	}

}