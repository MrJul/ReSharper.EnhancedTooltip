using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public static class SettingsExtensions {

		[NotNull]
		public static IContextBoundSettingsStore GetSettings([NotNull] this IDocument document)
			=> Shell.Instance
				.GetComponent<ISettingsStore>()
				.BindToContextTransient(ContextRange.Smart(document.ToDataContext()));

		[CanBeNull]
		public static IContextBoundSettingsStore TryGetSettings([CanBeNull] this IDocument document)
			=> document != null && Shell.HasInstance ? document.GetSettings() : null;

		public static bool IsEntryEqualToDefault<T>(
			[NotNull] this IContextBoundSettingsStore store,
			[NotNull] Expression<Func<IdentifierTooltipSettings, T>> settingsExpr)
		where T : struct
			=> store.Schema.GetScalarEntry(settingsExpr).GetDefaultValueInEntryMemberType() is T defaultValue
			&& EqualityComparer<T>.Default.Equals(store.GetValue(settingsExpr), defaultValue);

	}

}