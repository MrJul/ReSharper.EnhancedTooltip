using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.EnhancedTooltip.Presentation {

	/// <summary>
	/// Contains Visual Studio color names.
	/// </summary>
	public static class VsHighlightingAttributeIds {

		public const string Classes = "User Types";
		public const string Delegates = "User Types(Delegates)";
		public const string Enums = "User Types(Enums)";
		public const string Keyword = "Keyword";
		public const string Identifier = "Identifier";
		public const string Interfaces = "User Types(Interfaces)";
		public const string Number = "Number";
		public const string Operator = "Operator";
		public const string String = "String";
		public const string ValueTypes = "User Types(Value types)";
		
		[CanBeNull]
		public static string GetForTypeElement([CanBeNull] ITypeElement typeElement) {
			if (typeElement is IDelegate)
				return Delegates;
			if (typeElement is IEnum)
				return Enums;
			if (typeElement is IInterface)
				return Interfaces;
			if (typeElement is IStruct)
				return ValueTypes;
			if (typeElement is IClass)
				return Classes;
			// Note: there is an "User Types(Type parameters)" but it's actually never used by VS. It's even marked as "Won't Fix" on Connect.
			return null;
		}

	}

}