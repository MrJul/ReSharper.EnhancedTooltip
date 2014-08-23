using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// Provides colored identifier tooltips.
	/// </summary>
	[SolutionComponent]
	public class IdentifierTooltipContentProvider {

		private readonly ISolution _solution;
		private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		private readonly ColorizerPresenter _colorizerPresenter;

		/// <summary>
		/// Returns a colored <see cref="IdentifierContent"/> for an identifier represented by a <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter representing the identifier.</param>
		/// <param name="languageType">The type of language used to present the identifier.</param>
		/// <returns>A <see cref="IdentifierContent"/> representing a colored tooltip, or <c>null</c>.</returns>
		[CanBeNull]
		public IdentifierContent TryGetIdentifierText([NotNull] IHighlighter highlighter, [NotNull] PsiLanguageType languageType) {
			// Finds the element represented by the identifier.
			IPsiSourceFile psiSourceFile;
			DeclaredElementInstance elementInstance = FindValidHighlightedElement(highlighter, languageType, out psiSourceFile);
			if (elementInstance == null)
				return null;
			
			// Presents it using colors.
			var options = PresenterOptions.ForToolTip(psiSourceFile.GetSettingsStore());
			RichText identifierText = _colorizerPresenter.TryPresent(elementInstance, options, languageType, highlighter.AttributeId);
			if (identifierText == null || identifierText.IsEmpty)
				return null;

			IDeclaredElement element = elementInstance.Element;
			IPsiModule psiModule = psiSourceFile.PsiModule;

			return new IdentifierContent {
				Text = identifierText,
				Icon = TryGetIcon(element),
				Description = TryGetDescription(element, psiModule, languageType, DeclaredElementDescriptionStyle.NO_OBSOLETE_SUMMARY_STYLE),
				Obsolete = TryRemoveObsoletePrefix(TryGetDescription(element, psiModule, languageType, DeclaredElementDescriptionStyle.OBSOLETE_DESCRIPTION))
			};
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
		/// Finds a valid element represented by an <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter.</param>
		/// <param name="psiSourceFile">When the method returns, the <see cref="IPsiSourceFile"/> containing the highlighter.</param>
		/// <param name="languageType">The type of language used to present the identifier.</param>
		/// <returns>A valid <see cref="DeclaredElementInstance"/>, or <c>null</c>.</returns>
		[ContractAnnotation("=> null, psiSourceFile: null; => notnull, psiSourceFile: notnull")]
		[CanBeNull]
		private DeclaredElementInstance FindValidHighlightedElement([NotNull] IHighlighter highlighter, [NotNull] PsiLanguageType languageType,
			[CanBeNull] out IPsiSourceFile psiSourceFile) {
			psiSourceFile = null;

			if (!highlighter.IsValid || highlighter.AttributeId == HighlightingAttributeIds.MUTABLE_LOCAL_VARIABLE_IDENTIFIER_ATTRIBUTE)
				return null;

			IPsiServices psiServices = _solution.GetPsiServices();
			if (!psiServices.Files.AllDocumentsAreCommitted || psiServices.Caches.HasDirtyFiles)
				return null;

			psiSourceFile = highlighter.Document.GetPsiSourceFile(_solution);
			if (psiSourceFile == null || !psiSourceFile.IsValid())
				return null;

			var documentRange = new DocumentRange(highlighter.Document, highlighter.Range);
			IFile file = psiSourceFile.GetPsiFile(languageType, documentRange);
			if (file == null)
				return null;

			DeclaredElementInstance elementInstance = FindElement(file, documentRange);
			if (elementInstance == null || !elementInstance.IsValid())
				return null;

			return elementInstance;
		}

		/// <summary>
		/// Finds an element at a given file range, either a reference or a declaration.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <param name="range">The range to get the element in <paramref name="file"/>.</param>
		/// <returns>A <see cref="DeclaredElementInstance"/> at range <paramref name="range"/> in <paramref name="file"/>.</returns>
		[CanBeNull]
		private static DeclaredElementInstance FindElement([NotNull] IFile file, DocumentRange range) {
			TreeTextRange treeTextRange = file.Translate(range);
			if (!treeTextRange.IsValid())
				return null;

			// First finds a reference.
			IReference[] references = file.FindReferencesAt(treeTextRange);
			if (references.Length > 0)
				return GetBestReference(references);

			// Or a declaration.
			ITreeNode nodeAt = file.FindNodeAt(treeTextRange);
			if (nodeAt == null)
				return null;

			var containingNode = nodeAt.GetContainingNode<IDeclaration>(true);
			if (containingNode != null && containingNode.GetNameDocumentRange() == range) {
				IDeclaredElement declaredElement = containingNode.DeclaredElement;
				if (declaredElement != null)
					return new DeclaredElementInstance(declaredElement, EmptySubstitution.INSTANCE);
			}

			return null;
		}

		/// <summary>
		/// Gets the best reference (the "deepest" one) from a collection of references.
		/// </summary>
		/// <param name="references">A collection of references.</param>
		/// <returns>The <see cref="DeclaredElementInstance"/> corresponding to the best reference.</returns>
		[CanBeNull]
		private static DeclaredElementInstance GetBestReference([NotNull] IEnumerable<IReference> references) {
			foreach (IReference reference in references.OrderBy(r => r.GetTreeNode().PathToRoot().Count())) {
				IResolveResult resolveResult = reference.Resolve().Result;
				if (reference.CheckResolveResult() == ResolveErrorType.DYNAMIC)
					return null;

				IDeclaredElement foundElement = resolveResult.DeclaredElement;
				if (foundElement != null)
					return new DeclaredElementInstance(foundElement, resolveResult.Substitution);
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