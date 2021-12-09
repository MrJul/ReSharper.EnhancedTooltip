using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace GammaJul.ReSharper.EnhancedTooltip.DocumentMarkup {

	/// <summary>
	/// We can't directly reference ReSharper C++ or the plugin will fail for people not having it installed.
	/// Let's use some reflection instead.
	/// </summary>
	[SolutionComponent]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class ReflectionCppTooltipContentProvider {

		private const string ICppDeclaredElementTypeName = "JetBrains.ReSharper.Psi.Cpp.Language.ICppDeclaredElement, JetBrains.ReSharper.Psi.Cpp";
		private const string CppDeclaredElementTooltipProviderTypeName = "JetBrains.ReSharper.Feature.Services.Cpp.QuickDoc.CppDeclaredElementTooltipProvider, JetBrains.ReSharper.Feature.Services.Cpp";

		private readonly Lazy<Type?> _iCppDeclaredElementType;
		private readonly Lazy<Func<IDeclaredElement, RichTextBlock?>> _lazyGetTooltipMethod;

		private Func<IDeclaredElement, RichTextBlock?> CreateGetTooltipMethod(ISolution solution) {
			// Equivalent to element => _solution.TryGetComponent<CppDeclaredElementTooltipProvider>()?.GetTooltip(element, false);

			RichTextBlock? NoTooltip(IDeclaredElement element)
				=> null;

			var iCppDeclaredElementType = _iCppDeclaredElementType.Value;
			if (iCppDeclaredElementType is null)
				return NoTooltip;

			var cppDeclaredElementTooltipProviderType = Type.GetType(CppDeclaredElementTooltipProviderTypeName, throwOnError: false);
			if (cppDeclaredElementTooltipProviderType is null)
				return NoTooltip;

			Func<ISolution, object> tryGetComponentObject = SolutionEx.TryGetComponent<object>;
			MethodInfo tryGetComponent = tryGetComponentObject.Method.GetGenericMethodDefinition().MakeGenericMethod(cppDeclaredElementTooltipProviderType);

			object tooltipProvider = tryGetComponent.Invoke(null, new object[] { solution });
			if (tooltipProvider is null)
				return NoTooltip;

			var getTooltipMethod = cppDeclaredElementTooltipProviderType.GetMethod(
				"GetTooltip",
				BindingFlags.Instance | BindingFlags.Public,
				null,
				new[] { iCppDeclaredElementType, typeof(bool) },
				null);
			if (getTooltipMethod is null)
				return NoTooltip;

			return element => getTooltipMethod.Invoke(tooltipProvider, new object[] { element, false /* fillSpaces */ }) as RichTextBlock;
		}

		public bool IsCppDeclaredElement(IDeclaredElement declaredElement)
			=> _iCppDeclaredElementType.Value is { } iCppDeclaredElementType && iCppDeclaredElementType.IsInstanceOfType(declaredElement);

		public RichText? TryPresentCppDeclaredElement(IDeclaredElement declaredElement)
			=> _lazyGetTooltipMethod.Value(declaredElement)?.RichText;

		public ReflectionCppTooltipContentProvider(ISolution solution) {
			_iCppDeclaredElementType = Lazy.Of<Type?>(() => Type.GetType(ICppDeclaredElementTypeName, throwOnError: false), isThreadSafe: true);
			_lazyGetTooltipMethod = Lazy.Of(() => CreateGetTooltipMethod(solution), isThreadSafe: true);
		}

	}

}