using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GammaJul.ReSharper.EnhancedTooltip.Presentation;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Upgrade;
using JetBrains.ReSharper.Feature.Services.Lookup;
#pragma warning disable 618

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[MigrateSettings]
	public sealed class AnnotationsDisplayKindMigration : IMigrateSettings {

		public IEnumerable<SettingsEntry> GetEntriesToMigrate(ISettingsSchema schema) {
			yield return schema.GetEntry((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations);
			yield return schema.GetEntry((IdentifierTooltipSettings s) => s.ShowParametersAnnotations);
		}

		public IEnumerable<SettingsKey>? GetKeysToMigrate(ISettingsSchema schema)
			=> null;

		public void Migrate(IContextBoundSettingsStoreImplementation store) {
			MigrateValue(store, s => s.ShowIdentifierAnnotations, s => s.ShowIdentifierAttributes);
			MigrateValue(store, s => s.ShowParametersAnnotations, s => s.ShowParametersAttributes);
		}

		private static void MigrateValue(
			IContextBoundSettingsStore store,
			Expression<Func<IdentifierTooltipSettings, AnnotationsDisplayKind>> oldSettingExpr,
			Expression<Func<IdentifierTooltipSettings, AttributesDisplayKind>> newSettingExpr) {
			if (!store.IsEntryEqualToDefault(oldSettingExpr) && store.IsEntryEqualToDefault(newSettingExpr)) {
				AnnotationsDisplayKind annotationsDisplayKind = store.GetValue(oldSettingExpr);
				store.SetValue(newSettingExpr, annotationsDisplayKind.ToAttributesDisplayKind());
			}
		}

	}

}