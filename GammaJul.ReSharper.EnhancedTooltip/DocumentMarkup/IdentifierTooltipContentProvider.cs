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
			DocumentRange documentRange, [NotNull] IContextBoundSettingsStore settings, [CanBeNull] IHighlighter highlighter) {

			DeclaredElementInfo info = FindDeclaredElement(documentRange);
			if (info == null)
				return null;
			
			IdentifierTooltipContent standardContent = TryPresentColorized(info, settings)
				?? TryPresentNonColorized(highlighter, info.DeclaredElement, settings);

			bool replacesStandardContent;
			IdentifierTooltipContent additionalContent = TryGetAdditionalIdentifierContent(info, settings, out replacesStandardContent);
			if (replacesStandardContent) {
				standardContent = additionalContent;
				additionalContent = null;
			}

			var result = new IdentifierContentGroup();

			if (standardContent != null)
				result.Identifiers.Add(standardContent);
			if (additionalContent != null)
				result.Identifiers.Add(additionalContent);

			result.ArgumentRole = TryGetArgumentRoleContent(info.TreeNode, settings);

			return result;
		}

		[CanBeNull]
		private IdentifierTooltipContent TryGetAdditionalIdentifierContent(
			[NotNull] DeclaredElementInfo info,
			[NotNull] IContextBoundSettingsStore settings,
			out bool replacesStandardContent) {

			replacesStandardContent = false;

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
		private IdentifierTooltipContent TryPresentColorized([NotNull] DeclaredElementInfo info, [NotNull] IContextBoundSettingsStore settings) {

			PsiLanguageType languageType = info.TreeNode.Language;
			IDeclaredElement element = info.DeclaredElement;
			IPsiModule psiModule = info.TreeNode.GetPsiModule();

			HighlighterIdProvider highlighterIdProvider = _highlighterIdProviderFactory.CreateProvider(settings);

			RichText identifierText = _colorizerPresenter.TryPresent(
				new DeclaredElementInstance(element, info.Substitution),
				PresenterOptions.ForIdentifierToolTip(settings),
				languageType,
				highlighterIdProvider);

			if (identifierText == null || identifierText.IsEmpty)
				return null;
			
			var identifierContent = new IdentifierTooltipContent(identifierText, info.SourceRange) {
				Description = TryGetDescription(element, psiModule, languageType, DeclaredElementDescriptionStyle.NO_OBSOLETE_SUMMARY_STYLE),
			};

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
				identifierContent.Icon = TryGetIcon(element);

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowObsolete))
				identifierContent.Obsolete = TryRemoveObsoletePrefix(TryGetDescription(element, psiModule, languageType, DeclaredElementDescriptionStyle.OBSOLETE_DESCRIPTION));

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowExceptions))
				identifierContent.Exceptions.AddRange(GetExceptions(element, languageType, psiModule));

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowOverloadCount))
				identifierContent.OverloadCount = TryGetOverloadCountCount(element as IFunction, info.Reference, languageType);

			var typeElement = info.DeclaredElement as ITypeElement;
			if (typeElement != null) {

				bool showBaseType = settings.GetValue((IdentifierTooltipSettings s) => s.ShowBaseType);
				bool showImplementedInterfaces = settings.GetValue((IdentifierTooltipSettings s) => s.ShowImplementedInterfaces);
				if (showBaseType || showImplementedInterfaces)
					AddSuperTypes(identifierContent, typeElement, showBaseType, showImplementedInterfaces, languageType, highlighterIdProvider);

				if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowAttributesUsage) && typeElement.IsAttribute())
					identifierContent.AttributeUsage = GetAttributeUsage((IClass) info.DeclaredElement);

			}
			
			return identifierContent;
		}
		
		private static int? TryGetOverloadCountCount([CanBeNull] IFunction function, [CanBeNull] IReference reference, PsiLanguageType languageType) {
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
			bool showBaseType,
			bool showImplementedInterfaces,
			[NotNull] PsiLanguageType languageType,
			[NotNull] HighlighterIdProvider highlighterIdProvider) {

			DeclaredElementInstance baseType;
			IList<DeclaredElementInstance> implementedInterfaces;
			GetSuperTypes(typeElement, showBaseType, showImplementedInterfaces, out baseType, out implementedInterfaces);

			if (baseType != null)
				identifierContent.BaseType = _colorizerPresenter.TryPresent(baseType, PresenterOptions.QualifiedMember, languageType, highlighterIdProvider);

			if (implementedInterfaces.Count == 0)
				return;

			var sortedPresentedInterfaces = new SortedDictionary<string, RichText>(StringComparer.Ordinal);
			foreach (DeclaredElementInstance implementedInterface in implementedInterfaces) {
				RichText richText = _colorizerPresenter.TryPresent(implementedInterface, PresenterOptions.QualifiedMember, languageType, highlighterIdProvider);
				if (richText != null)
					sortedPresentedInterfaces[richText.ToString(false)] = richText;
			}
			foreach (RichText richText in sortedPresentedInterfaces.Values)
				identifierContent.ImplementedInterfaces.Add(richText);
		}

		private static void GetSuperTypes(
			[NotNull] ITypeElement typeElement,
			bool getBaseType,
			bool getImplementedInterfaces,
			[CanBeNull] out DeclaredElementInstance baseType,
			[NotNull] out IList<DeclaredElementInstance> implementedInterfaces) {

			baseType = null;
			implementedInterfaces = EmptyList<DeclaredElementInstance>.InstanceList;

			var searchForBaseType = getBaseType && typeElement is IClass;
			if (!searchForBaseType && !getImplementedInterfaces)
				return;

			var foundInterfaces = new LocalList<DeclaredElementInstance>();
			
			foreach (var superType in typeElement.GetAllSuperTypes()) {
				ITypeElement superTypeElement = superType.GetTypeElement();

				if (superTypeElement is IClass || superTypeElement is IDelegate) {
					if (searchForBaseType) {
						baseType = new DeclaredElementInstance(superTypeElement, superType.GetSubstitution());
						searchForBaseType = false;
						if (!getImplementedInterfaces)
							return;
					}
					continue;
				}

				if (getImplementedInterfaces && superTypeElement is IInterface)
					foundInterfaces.Add(new DeclaredElementInstance(superTypeElement, superType.GetSubstitution()));
			}

			implementedInterfaces = foundInterfaces.ResultingList();
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
		private IdentifierTooltipContent TryPresentNonColorized([CanBeNull] IHighlighter highlighter, [CanBeNull] IDeclaredElement element, [NotNull] IContextBoundSettingsStore settings) {
			RichTextBlock richTextToolTip = highlighter?.RichTextToolTip;
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

		[NotNull]
		private static IEnumerable<ExceptionContent> GetExceptions(
			[NotNull] IDeclaredElement element,
			[NotNull] PsiLanguageType languageType,
			[NotNull] IPsiModule psiModule) {

			XmlNode xmlDoc = element.GetXMLDoc(true);
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
				RichText richText = XmlDocRichTextPresenter.Run(exceptionElement, false, languageType, psiModule).RichText;
				if (!richText.IsNullOrEmpty())
					exceptionContent.Description = richText;
			}
			return exceptionContent;
		}

		/// <summary>
		/// Returns the description of an element, if available.
		/// </summary>
		/// <param name="element">The element whose description will be returned.</param>
		/// <param name="psiModule">The PSI module of the file containing the identifier.</param>
		/// <param name="languageType">The type of language used to present the identifier.</param>
		/// <param name="style">The description style to use.</param>
		[CanBeNull]
		private RichText TryGetDescription(
			[NotNull] IDeclaredElement element,
			[NotNull] IPsiModule psiModule,
			[NotNull] PsiLanguageType languageType,
			[NotNull] DeclaredElementDescriptionStyle style)
			=> _declaredElementDescriptionPresenter
				.GetDeclaredElementDescription(element, style, languageType, psiModule)
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
		
		/// <summary>
		/// Finds a valid element represented at a given <see cref="DocumentRange"/>.
		/// </summary>
		/// <param name="documentRange">The document range where to find a <see cref="IDeclaredElement"/>.</param>
		/// <returns>A valid <see cref="DeclaredElementInfo"/>, or <c>null</c>.</returns>
		[CanBeNull]
		private DeclaredElementInfo FindDeclaredElement(DocumentRange documentRange) {
			IDocument document = documentRange.Document;
			if (document == null || !documentRange.IsValid())
				return null;
			
			IPsiServices psiServices = _solution.GetPsiServices();
			if (!psiServices.Files.AllDocumentsAreCommitted || psiServices.Caches.HasDirtyFiles)
				return null;

			return document
				.GetPsiSourceFiles(_solution)
				.SelectMany(
					psiSourceFile => psiServices.Files.GetPsiFiles(psiSourceFile, documentRange),
					(psiSourceFile, file) => FindDeclaredElement(documentRange, file))
				.FirstOrDefault(info => info != null && info.DeclaredElement.IsValid());
		}

		/// <summary>
		/// Finds an element at a given file range, either a reference or a declaration.
		/// </summary>
		/// <param name="range">The range to get the element in <paramref name="file"/>.</param>
		/// <param name="file">The file to search into.</param>
		/// <returns>A <see cref="DeclaredElementInfo"/> at range <paramref name="range"/> in <paramref name="file"/>, or <c>null</c>.</returns>
		[CanBeNull]
		private static DeclaredElementInfo FindDeclaredElement(DocumentRange range, [NotNull] IFile file) {
			if (!file.IsValid())
				return null;

			TreeTextRange treeTextRange = file.Translate(range);
			if (!treeTextRange.IsValid())
				return null;

			IReference[] references = file.FindReferencesAt(treeTextRange);
			if (references.Length > 0)
				return GetBestReference(references);

			// FindNodeAt seems to return the previous node on single-char literals (eg '0'). FindNodesAt is fine.
			var node = file.FindNodesAt<ITreeNode>(treeTextRange).FirstOrDefault();
			if (node == null || !node.IsValid())
				return null;

			return FindDeclaration(node, file)
				?? FindConstant(node, file)
				?? FindSpecialElement(node, file);
		}

		/// <summary>
		/// Gets the best reference (the "deepest" one) from a collection of references.
		/// </summary>
		/// <param name="references">A collection of references.</param>
		/// <returns>The <see cref="DeclaredElementInfo"/> corresponding to the best reference.</returns>
		[CanBeNull]
		private static DeclaredElementInfo GetBestReference([NotNull] IEnumerable<IReference> references) {
			foreach (IReference reference in references.OrderBy(r => r.GetTreeNode().PathToRoot().Count())) {
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

		[CanBeNull]
		private static DeclaredElementInfo FindDeclaration([NotNull] ITreeNode node, [NotNull] IFile file) {
			var declaration = node.GetContainingNode<IDeclaration>(true);
			if (declaration == null)
				return null;

			TreeTextRange nameRange = declaration.GetNameRange();
			if (!nameRange.Intersects(node.GetTreeTextRange()))
				return null;

			IDeclaredElement declaredElement = declaration.DeclaredElement;
			if (declaredElement == null)
				return null;

			return new DeclaredElementInfo(declaredElement, EmptySubstitution.INSTANCE, node, file.GetDocumentRange(nameRange).TextRange, null);
		}

		[CanBeNull]
		private static DeclaredElementInfo FindConstant([NotNull] ITreeNode node, [NotNull] IFile file) {
			var literalExpression = node.GetContainingNode<ILiteralExpression>(true);
			if (literalExpression == null)
				return null;

			TreeTextRange literalRange = literalExpression.Literal.GetTreeTextRange();
			if (!literalRange.Intersects(node.GetTreeTextRange()))
				return null;

			var declaredType = literalExpression.ConstantValue.Type as IDeclaredType;
			ITypeElement typeElement = declaredType?.GetTypeElement();
			if (typeElement == null)
				return null;

			return new DeclaredElementInfo(typeElement, declaredType.GetSubstitution(), node, file.GetDocumentRange(literalRange).TextRange, null);
		}

		[CanBeNull]
		private static DeclaredElementInfo FindSpecialElement([NotNull] ITreeNode node, [NotNull] IFile file) {
			var finder = LanguageManager.Instance.TryGetService<ISpecialDeclaredElementFinder>(file.Language);
			if (finder == null)
				return null;

			TextRange sourceRange;
			DeclaredElementInstance declaredElementInstance = finder.FindDeclaredElement(node, file, out sourceRange);
			if (declaredElementInstance == null)
				return null;

			return new DeclaredElementInfo(declaredElementInstance.Element, declaredElementInstance.Substitution, node, sourceRange, null);
		}

		
		[CanBeNull]
		private ArgumentRoleTooltipContent TryGetArgumentRoleContent([NotNull] ITreeNode node, [NotNull] IContextBoundSettingsStore settings) {
			if (!settings.GetValue((IdentifierTooltipSettings s) => s.ShowArgumentsRole))
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
			final.Append(_colorizerPresenter.TryPresent(
				new DeclaredElementInstance(parametersOwner, parameterInstance.Substitution),
				PresenterOptions.ForArgumentRoleParametersOwnerToolTip(settings),
				argument.Language,
				highlighterIdProvider));
			final.Append(": ", TextStyle.Default);
			final.Append(_colorizerPresenter.TryPresent(
				parameterInstance,
				PresenterOptions.ForArgumentRoleParameterToolTip(settings),
				argument.Language,
				highlighterIdProvider));

			var content = new ArgumentRoleTooltipContent(final, argument.GetDocumentRange().TextRange) {
				Description = TryGetDescription(parameter, parameter.Module, argument.Language, DeclaredElementDescriptionStyle.NO_OBSOLETE_SUMMARY_STYLE)
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