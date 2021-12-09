using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Resources.Shell;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	public static class SettingsExtensions {

		public static IContextBoundSettingsStore GetSettings(this IDocument document)
			=> Shell.Instance
				.GetComponent<ISettingsStore>()
				.BindToContextTransient(ContextRange.Smart(document.ToDataContext()));

		public static IContextBoundSettingsStore? TryGetSettings(this IDocument? document)
			=> document is not null && Shell.HasInstance ? document.GetSettings() : null;

		public static bool IsEntryEqualToDefault<T>(
			this IContextBoundSettingsStore store,
			Expression<Func<IdentifierTooltipSettings, T>> settingsExpr)
		where T : struct
			=> store.Schema.GetScalarEntry(settingsExpr).GetDefaultValueInEntryMemberType() is T defaultValue
			&& EqualityComparer<T>.Default.Equals(store.GetValue(settingsExpr), defaultValue);

	}

}