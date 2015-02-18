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
#if RS90
using JetBrains.ReSharper.Feature.Services.Daemon;
#elif RS82
using JetBrains.ReSharper.Daemon;
#endif

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// Provides colored identifier tooltips.
	/// </summary>
	[SolutionComponent]
	public class IdentifierTooltipContentProvider {
		
		private sealed class DeclaredElementInfo {

			[NotNull] internal readonly IDeclaredElement DeclaredElement;
			[NotNull] internal readonly ISubstitution Substitution;
			[NotNull] internal readonly IPsiSourceFile PsiSourceFile;
			internal readonly TextRange SourceRange;
			[CanBeNull] internal readonly IReference Reference;

			public DeclaredElementInfo([NotNull] IDeclaredElement declaredElement, [NotNull] ISubstitution substitution, [NotNull] IPsiSourceFile psiSourceFile,
				TextRange sourceRange, [CanBeNull] IReference reference) {
				DeclaredElement = declaredElement;
				Substitution = substitution;
				PsiSourceFile = psiSourceFile;
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
		[CanBeNull]
		public IdentifierTooltipContent TryGetIdentifierContent([NotNull] IHighlighter highlighter, [NotNull] IContextBoundSettingsStore settings) {
			if (!highlighter.IsValid || !settings.GetValue((IdentifierTooltipSettings s) => s.Enabled))
				return null;

			var documentRange = new DocumentRange(highlighter.Document, highlighter.Range);

			DeclaredElementInfo info = FindDeclaredElement(documentRange);
			if (info == null)
				return null;
			
			return TryPresentColorized(info, settings)
				?? TryPresentNonColorized(highlighter, info.DeclaredElement, settings);
		}

		/// <summary>
		/// Returns a colored <see cref="IdentifierTooltipContent"/> for an identifier at a given <see cref="DocumentRange"/>.
		/// </summary>
		/// <param name="documentRange">The document range where to find a <see cref="IDeclaredElement"/>.</param>
		/// <param name="settings">The settings to use.</param>
		/// <returns>A <see cref="IdentifierTooltipContent"/> representing a colored tooltip, or <c>null</c>.</returns>
		[CanBeNull]
		public IdentifierTooltipContent TryGetIdentifierContent(DocumentRange documentRange, [NotNull] IContextBoundSettingsStore settings) {
			if (!settings.GetValue((IdentifierTooltipSettings s) => s.Enabled))
				return null;

			DeclaredElementInfo info = FindDeclaredElement(documentRange);
			if (info == null)
				return null;

			return TryPresentColorized(info, settings);
		}

		[CanBeNull]
		private IdentifierTooltipContent TryPresentColorized([NotNull] DeclaredElementInfo info, [NotNull] IContextBoundSettingsStore settings) {
			PsiLanguageType languageType = info.PsiSourceFile.PrimaryPsiLanguage;
			IDeclaredElement element = info.DeclaredElement;
			IPsiModule psiModule = info.PsiSourceFile.PsiModule;

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
				identifierContent.Exceptions.AddRange(GetExceptions(element, languageType, psiModule, info.PsiSourceFile.ResolveContext));

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
		private IdentifierTooltipContent TryPresentNonColorized([NotNull] IHighlighter highlighter, [NotNull] IDeclaredElement element, [NotNull] IContextBoundSettingsStore settings) {
			RichTextBlock richTextToolTip = highlighter.RichTextToolTip;
			if (richTextToolTip == null)
				return null;

			RichText richText = richTextToolTip.RichText;
			if (richText.IsNullOrEmpty())
				return null;

			var identifierContent = new IdentifierTooltipContent(richText, highlighter.Range);
			if (settings.GetValue((IdentifierTooltipSettings s) => s.ShowIcon))
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
			
			foreach (IPsiSourceFile psiSourceFile in document.GetPsiSourceFiles(_solution)) {
				foreach (IFile file in psiServices.Files.GetPsiFiles(psiSourceFile, documentRange)) {
					DeclaredElementInfo info = FindDeclaredElement(documentRange, psiSourceFile, file);
					if (info != null && info.DeclaredElement.IsValid())
						return info;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds an element at a given file range, either a reference or a declaration.
		/// </summary>
		/// <param name="range">The range to get the element in <paramref name="file"/>.</param>
		/// <param name="psiSourceFile">The PSI source file containing the range.</param>
		/// <param name="file">The file to search into.</param>
		/// <returns>A <see cref="DeclaredElementInfo"/> at range <paramref name="range"/> in <paramref name="file"/>, or <c>null</c>.</returns>
		[CanBeNull]
		private static DeclaredElementInfo FindDeclaredElement(DocumentRange range, [NotNull] IPsiSourceFile psiSourceFile, [NotNull] IFile file) {
			if (!file.IsValid())
				return null;

			TreeTextRange treeTextRange = file.Translate(range);
			if (!treeTextRange.IsValid())
				return null;

			// First finds a reference.
			IReference[] references = file.FindReferencesAt(treeTextRange);
			if (references.Length > 0)
				return GetBestReference(references, psiSourceFile);

			// Or a declaration.
			ITreeNode nodeAt = file.FindNodeAt(treeTextRange);
			if (nodeAt == null)
				return null;

			var containingNode = nodeAt.GetContainingNode<IDeclaration>(true);
			if (containingNode == null)
				return null;

			DocumentRange nameRange = containingNode.GetNameDocumentRange();
			if (!nameRange.Intersects(range))
				return null;

			IDeclaredElement declaredElement = containingNode.DeclaredElement;
			if (declaredElement == null)
				return null;

			return new DeclaredElementInfo(declaredElement, EmptySubstitution.INSTANCE, psiSourceFile, nameRange.TextRange, null);
		}

		/// <summary>
		/// Gets the best reference (the "deepest" one) from a collection of references.
		/// </summary>
		/// <param name="references">A collection of references.</param>
		/// <param name="psiSourceFile">The PSI source file containing the references.</param>
		/// <returns>The <see cref="DeclaredElementInfo"/> corresponding to the best reference.</returns>
		[CanBeNull]
		private static DeclaredElementInfo GetBestReference([NotNull] IEnumerable<IReference> references, [NotNull] IPsiSourceFile psiSourceFile) {
			foreach (IReference reference in references.OrderBy(r => r.GetTreeNode().PathToRoot().Count())) {
				IResolveResult resolveResult = reference.Resolve().Result;
				if (reference.CheckResolveResult() == ResolveErrorType.DYNAMIC)
					return null;

				IDeclaredElement foundElement = resolveResult.DeclaredElement;
				if (foundElement != null) {
					var referenceRange = reference.GetDocumentRange().TextRange;
					return new DeclaredElementInfo(foundElement, resolveResult.Substitution, psiSourceFile, referenceRange, reference);
				}
			}

			return null;
		}

		public IdentifierTooltipContentProvider([NotNull] ISolution solution, [NotNull] ColorizerPresenter colorizerPresenter,
			[NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter) {
			_solution = solution;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}