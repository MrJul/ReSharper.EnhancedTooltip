using System.Linq.Expressions;
using JetBrains.Util.dataStructures;
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
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
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
			[NotNull] internal readonly IFile File;
			internal readonly TextRange SourceRange;
			[CanBeNull] internal readonly IReference Reference;
			
			public DeclaredElementInfo([NotNull] IDeclaredElement declaredElement, [NotNull] ISubstitution substitution, [NotNull] IFile file,
				TextRange sourceRange, [CanBeNull] IReference reference) {
				DeclaredElement = declaredElement;
				Substitution = substitution;
				File = file;
				SourceRange = sourceRange;
				Reference = reference;
			}

		}

		[NotNull] private readonly ISolution _solution;
		[NotNull] private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		[NotNull] private readonly ColorizerPresenter _colorizerPresenter;
		
		/// <summary>
		/// Returns a colored <see cref="IdentifierTooltipContent"/> for an identifier represented by a <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter representing the identifier.</param>
		/// <param name="settings">The settings to use.</param>
		/// <returns>A <see cref="IdentifierTooltipContent"/> representing a colored tooltip, or <c>null</c>.</returns>
		[NotNull]
		public IdentifierTooltipContent[] GetIdentifierContents([NotNull] IHighlighter highlighter, [NotNull] IContextBoundSettingsStore settings) {
			if (!highlighter.IsValid)
				return EmptyArray<IdentifierTooltipContent>.Instance;

			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled)) {
				IdentifierTooltipContent content = TryPresentNonColorized(highlighter, null, settings);
				if (content != null)
					return new[] { content };
				return EmptyArray<IdentifierTooltipContent>.Instance;
			}

			return GetIdentifierContentsCore(new DocumentRange(highlighter.Document, highlighter.Range), settings, highlighter);
		}

		/// <summary>
		/// Returns a colored <see cref="IdentifierTooltipContent"/> for an identifier at a given <see cref="DocumentRange"/>.
		/// </summary>
		/// <param name="documentRange">The document range where to find a <see cref="IDeclaredElement"/>.</param>
		/// <param name="settings">The settings to use.</param>
		/// <returns>A <see cref="IdentifierTooltipContent"/> representing a colored tooltip, or <c>null</c>.</returns>
		[NotNull]
		public IdentifierTooltipContent[] GetIdentifierContents(DocumentRange documentRange, [NotNull] IContextBoundSettingsStore settings) {
			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled))
				return EmptyArray<IdentifierTooltipContent>.Instance;

			return GetIdentifierContentsCore(documentRange, settings, null);
		}

		[NotNull]
		private IdentifierTooltipContent[] GetIdentifierContentsCore(
			DocumentRange documentRange, [NotNull] IContextBoundSettingsStore settings, [CanBeNull] IHighlighter highlighter) {

			DeclaredElementInfo info = FindDeclaredElement(documentRange);
			if (info == null)
				return EmptyArray<IdentifierTooltipContent>.Instance;

			IdentifierTooltipContent standardContent = TryPresentColorized(info, settings)
				?? TryPresentNonColorized(highlighter, info.DeclaredElement, settings);

			bool replacesStandardContent;
			IdentifierTooltipContent additionalContent = TryGetAdditionalIdentifierContent(info, settings, out replacesStandardContent);
			if (replacesStandardContent) {
				standardContent = additionalContent;
				additionalContent = null;
			}

			var results = new FrugalLocalList<IdentifierTooltipContent>();
			if (standardContent != null)
				results.Add(standardContent);
			if (additionalContent != null)
				results.Add(additionalContent);
			return results.ToArray();
		}

		[CanBeNull]
		private IdentifierTooltipContent TryGetAdditionalIdentifierContent([NotNull] DeclaredElementInfo info, [NotNull] IContextBoundSettingsStore settings,
			out bool replacesStandardContent) {

			replacesStandardContent = false;

			var constructor = info.DeclaredElement as IConstructor;
			if (constructor == null)
				return null;

			ITypeElement typeElement = constructor.GetContainingType();
			if (typeElement == null)
				return null;

			IDeclaredType attributeType = info.File.GetPsiModule().GetPredefinedType(constructor.ResolveContext).Attribute;
			var settingsKey = GetConstructorSettingsKey(typeElement, info.Substitution, attributeType);
			ConstructorReferenceDisplay display = settings.GetValue(settingsKey);
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
		private static Expression<Func<IdentifierTooltipSettings, ConstructorReferenceDisplay>> GetConstructorSettingsKey(
			[NotNull] ITypeElement typeElement, [NotNull] ISubstitution substitution, [NotNull] IType attributeType) {

			if (TypeFactory.CreateType(typeElement, substitution).IsSubtypeOf(attributeType))
				return s => s.AttributeConstructorReferenceDisplay;
			return s => s.ConstructorReferenceDisplay;
		}

		[CanBeNull]
		private IdentifierTooltipContent TryGetTypeIdentifierContentFromConstructor(
			[NotNull] IConstructor constructor, [NotNull] DeclaredElementInfo constructorInfo, [NotNull] IContextBoundSettingsStore settings) {

			ITypeElement typeElement = constructor.GetContainingType();
			if (typeElement == null)
				return null;

			var typeInfo = new DeclaredElementInfo(typeElement, constructorInfo.Substitution, constructorInfo.File, constructorInfo.SourceRange, null);
			return TryPresentColorized(typeInfo, settings);
		}

		[CanBeNull]
		private IdentifierTooltipContent TryPresentColorized([NotNull] DeclaredElementInfo info, [NotNull] IContextBoundSettingsStore settings) {
			PsiLanguageType languageType = info.File.Language;
			IDeclaredElement element = info.DeclaredElement;
			IPsiModule psiModule = info.File.GetPsiModule();

			RichText identifierText = _colorizerPresenter.TryPresent(
				new DeclaredElementInstance(element, info.Substitution),
				PresenterOptions.ForToolTip(settings),
				languageType,
				settings.GetValue(HighlightingSettingsAccessor.IdentifierHighlightingEnabled));

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
				identifierContent.Exceptions.AddRange(GetExceptions(element, languageType, psiModule, info.File.GetResolveContext()));

			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowOverloadCount))
				identifierContent.OverloadCount = TryGetOverloadCountCount(element as IFunction, info.Reference, languageType);

			return identifierContent;
		}

		private static int? TryGetOverloadCountCount([CanBeNull] IFunction function, [CanBeNull] IReference reference, PsiLanguageType languageType) {
			if (function == null || reference == null)
				return null;

			var candidateCountProvider = LanguageManager.Instance.TryGetService<IInvocationCandidateCountProvider>(languageType);
			if (candidateCountProvider == null)
				return null;

			int? candidateCount = candidateCountProvider.TryGetInvocationCandidateCount(reference);
			if (candidateCount == null || candidateCount.Value <= 1)
				return null;

			return candidateCount.Value - 1;
		}
		
		[CanBeNull]
		private IdentifierTooltipContent TryPresentNonColorized([CanBeNull] IHighlighter highlighter, [CanBeNull] IDeclaredElement element, [NotNull] IContextBoundSettingsStore settings) {
			if (highlighter == null)
				return null;

			RichTextBlock richTextToolTip = highlighter.RichTextToolTip;
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
		private static IEnumerable<ExceptionContent> GetExceptions([NotNull] IDeclaredElement element, [NotNull] PsiLanguageType languageType,
			[NotNull] IPsiModule psiModule, [NotNull] IModuleReferenceResolveContext resolveContext) {
			XmlNode xmlDoc = element.GetXMLDoc(true);
			if (xmlDoc == null)
				return EmptyList<ExceptionContent>.InstanceList;

			XmlNodeList exceptionNodes = xmlDoc.SelectNodes("exception");
			if (exceptionNodes == null || exceptionNodes.Count == 0)
				return EmptyList<ExceptionContent>.InstanceList;

			var exceptions = new LocalList<ExceptionContent>();
			foreach (XmlNode exceptionNode in exceptionNodes) {
				ExceptionContent exceptionContent = TryExtractException(exceptionNode as XmlElement, languageType, psiModule, resolveContext);
				if (exceptionContent != null)
					exceptions.Add(exceptionContent);
			}
			return exceptions.ResultingList();
		}

		[CanBeNull]
		private static ExceptionContent TryExtractException([CanBeNull] XmlElement exceptionElement, [NotNull] PsiLanguageType languageType,
			[NotNull] IPsiModule psiModule, IModuleReferenceResolveContext resolveContext) {
			if (exceptionElement == null)
				return null;

			string cref = exceptionElement.GetAttribute("cref");
			if (String.IsNullOrEmpty(cref))
				return null;

			cref = XmlDocPresenterUtil.ProcessCref(cref);
			if (String.IsNullOrEmpty(cref))
				return null;

			var exceptionContent = new ExceptionContent { Exception = cref };
			if (exceptionElement.HasChildNodes) {
				RichText richText = XmlDocRichTextPresenter.Run(exceptionElement, false, languageType, psiModule, resolveContext).RichText;
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
		/// <param name="style"></param>
		[CanBeNull]
		private RichText TryGetDescription([NotNull] IDeclaredElement element, [NotNull] IPsiModule psiModule, [NotNull] PsiLanguageType languageType,
			[NotNull] DeclaredElementDescriptionStyle style) {
			RichTextBlock description = _declaredElementDescriptionPresenter.GetDeclaredElementDescription(element, style, languageType, psiModule);
			return description != null ? description.RichText : null;
		}

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
				return GetBestReference(references, file);

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
		/// <param name="file">The PSI file containing the references.</param>
		/// <returns>The <see cref="DeclaredElementInfo"/> corresponding to the best reference.</returns>
		[CanBeNull]
		private static DeclaredElementInfo GetBestReference([NotNull] IEnumerable<IReference> references, [NotNull] IFile file) {
			foreach (IReference reference in references.OrderBy(r => r.GetTreeNode().PathToRoot().Count())) {
				IResolveResult resolveResult = reference.Resolve().Result;
				if (reference.CheckResolveResult() == ResolveErrorType.DYNAMIC)
					return null;

				IDeclaredElement foundElement = resolveResult.DeclaredElement;
				if (foundElement != null) {
					var referenceRange = reference.GetDocumentRange().TextRange;
					return new DeclaredElementInfo(foundElement, resolveResult.Substitution, file, referenceRange, reference);
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

			return new DeclaredElementInfo(declaredElement, EmptySubstitution.INSTANCE, file, file.GetDocumentRange(nameRange).TextRange, null);
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
			if (declaredType == null)
				return null;

			ITypeElement typeElement = declaredType.GetTypeElement();
			if (typeElement == null)
				return null;

			return new DeclaredElementInfo(typeElement, declaredType.GetSubstitution(), file, file.GetDocumentRange(literalRange).TextRange, null);
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

			return new DeclaredElementInfo(declaredElementInstance.Element, declaredElementInstance.Substitution, file, sourceRange, null);
		}

		public IdentifierTooltipContentProvider([NotNull] ISolution solution, [NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter) {
			_solution = solution;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}