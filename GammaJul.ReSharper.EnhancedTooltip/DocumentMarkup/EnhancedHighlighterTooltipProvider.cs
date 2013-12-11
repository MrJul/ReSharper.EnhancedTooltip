using System.Collections.Generic;
using System.Linq;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// An implementation of <see cref="IHighlighterTooltipProvider"/> that provides colored C# identifier tooltips,
	/// or falls back to an underlying provider when necessary.
	/// </summary>
	public class EnhancedHighlighterTooltipProvider : IHighlighterTooltipProvider {

		private readonly IHighlighterTooltipProvider _underlyingHighlighterTooltipProvider;
		private readonly ISolution _solution;
		private readonly IDeclaredElementDescriptionPresenter _declaredElementDescriptionPresenter;
		private readonly ColorizerPresenter _colorizerPresenter;

		public RichTextBlock GetRichTooltip(IHighlighter highlighter) {
			// For C# identifiers, colorize the tooltip.
			var csProvider = _underlyingHighlighterTooltipProvider as IdentifierTooltipProvider<CSharpLanguage>;
			if (csProvider != null) {
				RichTextBlock coloredTooltip = GetCSharpColoredTooltip(highlighter);
				if (coloredTooltip != null)
					return coloredTooltip;
			}

			// Fallback to the underlying provider.
			return _underlyingHighlighterTooltipProvider.GetRichTooltip(highlighter);
		}

		public string GetTooltip(IHighlighter highlighter) {
			return _underlyingHighlighterTooltipProvider.GetTooltip(highlighter);
		}

		public string GetTooltipForErrorStripe(IHighlighter highlighter) {
			return _underlyingHighlighterTooltipProvider.GetTooltipForErrorStripe(highlighter);
		}

		/// <summary>
		/// Returns a colored <see cref="RichTextBlock"/> for a C# identifier represented by a <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter representing the identifier.</param>
		/// <returns>A <see cref="RichTextBlock"/> representing a colored tooltip, or <c>null</c>.</returns>
		[CanBeNull]
		private RichTextBlock GetCSharpColoredTooltip([NotNull] IHighlighter highlighter) {
			// Finds the element represented by the identifier.
			IPsiSourceFile psiSourceFile;
			DeclaredElementInstance elementInstance = FindValidHighlightedElement(highlighter, out psiSourceFile);
			if (elementInstance == null)
				return null;
			
			// Presents it using colors.
			var options = new PresenterOptions(psiSourceFile.GetSettingsStore()) { ShowElementKind = true };
			RichText richText = _colorizerPresenter.Present(elementInstance, options, highlighter.AttributeId);
			if (richText.IsEmpty)
				return null;

			// Appends the element's description.
			var richTextBlock = new RichTextBlock(richText);
			AppendDescription(richTextBlock, elementInstance.Element, psiSourceFile.PsiModule);
			return richTextBlock;
		}

		/// <summary>
		/// Appends the description of an element to a <see cref="RichTextBlock"/>.
		/// </summary>
		/// <param name="richTextBlock">The block to append to.</param>
		/// <param name="element">The element whose description will be appended.</param>
		/// <param name="psiModule">The PSI module of the file containing the identifier.</param>
		private void AppendDescription([NotNull] RichTextBlock richTextBlock, [NotNull] IDeclaredElement element, [NotNull] IPsiModule psiModule) {
			RichTextBlock description = _declaredElementDescriptionPresenter.GetDeclaredElementDescription(element,
				DeclaredElementDescriptionStyle.SUMMARY_STYLE, CSharpLanguage.Instance, psiModule);

			if (!RichTextBlock.IsNullOrEmpty(description)) {
				richTextBlock.Add(new RichText());
				richTextBlock.AddLines(description);
			}
		}

		/// <summary>
		/// Finds a valid element represented by an <see cref="IHighlighter"/>.
		/// </summary>
		/// <param name="highlighter">The highlighter.</param>
		/// <param name="psiSourceFile">When the method returns, the <see cref="IPsiSourceFile"/> containing the highlighter.</param>
		/// <returns>A valid <see cref="DeclaredElementInstance"/>, or <c>null</c>.</returns>
		[ContractAnnotation("=> null, psiSourceFile: null; => notnull, psiSourceFile: notnull")]
		[CanBeNull]
		private DeclaredElementInstance FindValidHighlightedElement([NotNull] IHighlighter highlighter, [CanBeNull] out IPsiSourceFile psiSourceFile) {
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
			IFile file = psiSourceFile.GetPsiFile<CSharpLanguage>(documentRange);
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

		public EnhancedHighlighterTooltipProvider([NotNull] IHighlighterTooltipProvider underlyingHighlighterTooltipProvider, [NotNull] ISolution solution,
			[NotNull] IDeclaredElementDescriptionPresenter declaredElementDescriptionPresenter,
			[NotNull] ColorizerPresenter colorizerPresenter) {
			_underlyingHighlighterTooltipProvider = underlyingHighlighterTooltipProvider;
			_solution = solution;
			_declaredElementDescriptionPresenter = declaredElementDescriptionPresenter;
			_colorizerPresenter = colorizerPresenter;
		}

	}

}