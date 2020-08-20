using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Upgrade;
#pragma warning disable 618

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[MigrateSettings]
	public sealed class BaseTypeAndImplementedInterfacesDisplayKindMigration : IMigrateSettings {

		public IEnumerable<SettingsEntry> GetEntriesToMigrate(ISettingsSchema schema) {
			yield return schema.GetEntry((IdentifierTooltipSettings s) => s.ShowBaseType);
			yield return schema.GetEntry((IdentifierTooltipSettings s) => s.ShowImplementedInterfaces);
		}

		public IEnumerable<SettingsKey> GetKeysToMigrate(ISettingsSchema schema)
			=> null;

		public void Migrate(IContextBoundSettingsStoreImplementation store) {
			MigrateValue(store, s => s.ShowBaseType, s => s.BaseTypeDisplayKind, BaseTypeDisplayKind.Always, BaseTypeDisplayKind.Never);
			MigrateValue(store, s => s.ShowImplementedInterfaces, s => s.ImplementedInterfacesDisplayKind, ImplementedInterfacesDisplayKind.Always, ImplementedInterfacesDisplayKind.Never);
		}

		private static void MigrateValue<TDisplayKind>(
			[NotNull] IContextBoundSettingsStore store,
			[NotNull] Expression<Func<IdentifierTooltipSettings, bool>> oldSettingExpr,
			[NotNull] Expression<Func<IdentifierTooltipSettings, TDisplayKind>> newSettingExpr,
			TDisplayKind trueValue,
			TDisplayKind falseValue)
		where TDisplayKind : struct {
			if (!store.IsEntryEqualToDefault(oldSettingExpr) && store.IsEntryEqualToDefault(newSettingExpr)) {
				TDisplayKind displayKind = store.GetValue(oldSettingExpr) ? trueValue : falseValue;
				store.SetValue(newSettingExpr, displayKind);
			}
		}

	}

}