using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Upgrade;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[MigrateSettings]
	public sealed class BaseTypeAndImplementedInterfacesDisplayKindMigration : IMigrateSettings {

		public IEnumerable<SettingsEntry> GetEntriesToMigrate(ISettingsSchema schema) {
			yield return schema.GetEntry((IdentifierTooltipSettings s) => s.ShowBaseType);
			yield return schema.GetEntry((IdentifierTooltipSettings s) => s.ShowImplementedInterfaces);
		}

		public IEnumerable<SettingsKey> GetKeysToMigrate(ISettingsSchema schema)
			=> null;

		public void Migrate(IContextBoundSettingsStore store) {
			MigrateValue(store, s => s.ShowBaseType, s => s.BaseTypeDisplayKind);
			MigrateValue(store, s => s.ShowImplementedInterfaces, s => s.ImplementedInterfacesDisplayKind);
		}

		private static void MigrateValue(
			[NotNull] IContextBoundSettingsStore store,
			[NotNull] Expression<Func<IdentifierTooltipSettings, bool>> oldSettingExpr,
			[NotNull] Expression<Func<IdentifierTooltipSettings, BaseTypeDisplayKind>> newSettingExpr) {

			if (!IsEntryEqualToDefault(store, oldSettingExpr) && IsEntryEqualToDefault(store, newSettingExpr)) {
				var displayKind = store.GetValue(oldSettingExpr) ? BaseTypeDisplayKind.Always : BaseTypeDisplayKind.Never;
				store.SetValue(newSettingExpr, displayKind);
			}
		}

		private static bool IsEntryEqualToDefault<T>(
			[NotNull] IContextBoundSettingsStore store,
			[NotNull] Expression<Func<IdentifierTooltipSettings, T>> settingsExpr)
		where T : struct {
			object defaultValue = store.Schema.GetScalarEntry(settingsExpr).GetDefaultValueInEntryMemberType();
			return defaultValue is T && EqualityComparer<T>.Default.Equals(store.GetValue(settingsExpr), (T) defaultValue);
		}

	}

}