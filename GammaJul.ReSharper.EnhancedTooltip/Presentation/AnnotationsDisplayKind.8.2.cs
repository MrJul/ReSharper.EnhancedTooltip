using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace JetBrains.ReSharper.Feature.Services.Lookup {

	// Defined in ReSharper 9
	public enum AnnotationsDisplayKind {
		[Description("Do not display")] None,
		[Description("Display [NotNull] and [CanBeNull] only")] Nullness,
		[Description("Display all")] All,
	}

}