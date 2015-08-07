using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Intellisense.Options;
using JetBrains.UI.Controls;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Extensions;
using JetBrains.UI.Options;
using JetBrains.UI.Wpf.Controls;
using JetBrains.UI.Wpf.Converters;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[OptionsPage(Pid, "Enhanced Tooltip", typeof(ThemedIcons.Logo16), ParentId = IntelliSensePage.PID, Sequence = 100)]
	public partial class EnhancedTooltipOptionsPage : IOptionsPage {

		public const string Pid = "EnhancedTooltip.OptionsPage";

		[NotNull] private readonly Lifetime _lifetime;
		[NotNull] private readonly OptionsSettingsSmartContext _context;

		public string Id
			=> Pid;

		public bool OnOk()
			=> true;

		public bool ValidatePage()
			=> true;

		public EitherControl Control
			=> this;

		[NotNull]
		private static List<EnumValue> CreateEnumItemsSource([NotNull] Type enumType) {
			var enumDescriptionConverter = new EnumDescriptionConverter();
			return Enum.GetValues(enumType)
				.Cast<Enum>()
				.Select(e => new EnumValue(e, enumDescriptionConverter.Convert(e, typeof(string), null, CultureInfo.CurrentUICulture) as string))
				.ToList();
		}

		private void SetCheckBoxBinding<TSettings>(
			[NotNull] Expression<Func<TSettings, bool>> settingAccessor,
			[NotNull] CheckBoxDisabledNoCheck2 checkBox,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckBox,
			bool addColonToDescription = false) {

			var entry = _context.Schema.GetScalarEntry(settingAccessor);
			if (checkBox.Content == null) {
				string description = entry.Description;
				if (addColonToDescription)
					description += ":";
				checkBox.Content = description;
			}
			_context.SetBinding<bool>(_lifetime, entry, checkBox, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

			parentCheckBox?.IsCheckedLogically.FlowInto(_lifetime, checkBox, IsEnabledProperty);
		}

		private void SetComboBoxBinding<TSettings, TEnum>(
			[NotNull] Expression<Func<TSettings, TEnum>> settingAccessor,
			[NotNull] ComboBox comboBox,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckBox) {

			comboBox.ItemsSource = CreateEnumItemsSource(typeof(TEnum));
			comboBox.DisplayMemberPath = nameof(EnumValue.Description);
			comboBox.SelectedValuePath = nameof(EnumValue.Value);

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			_context.SetBinding<object>(_lifetime, entry, comboBox, Selector.SelectedValueProperty);

			SetAssociatedLabel(comboBox, entry.Description);
			
			parentCheckBox?.IsCheckedLogically.FlowInto(_lifetime, comboBox, IsEnabledProperty);
		}

		private void SetNumericUpDownBinding<TSettings>(
			[NotNull] Expression<Func<TSettings, int>> settingAccessor,
			[NotNull] NumericUpDown numericUpDown,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckBox) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			_context.SetBinding<int>(_lifetime, entry, numericUpDown, NumericUpDown.ValueProperty);

			SetAssociatedLabel(numericUpDown, entry.Description);

			parentCheckBox?.IsCheckedLogically.FlowInto(_lifetime, numericUpDown, IsEnabledProperty);
		}

		private static void SetAssociatedLabel([NotNull] FrameworkElement element, [NotNull] string description) {
			Label label = element.Parent?.FindDescendant<Label>();
			if (label != null && label.Content == null)
				label.Content = description + ":";
		}

		private void SetIdentifierTooltipSettingsBindings() {
			CheckBoxDisabledNoCheck2 enabledCheckBox = IdentifierTooltipEnabledCheckBox;
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.Enabled, enabledCheckBox, null);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowIcon, IdentifierTooltipShowIconCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowKind, IdentifierTooltipShowKindCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowObsolete, IdentifierTooltipShowObsoleteCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowExceptions, IdentifierTooltipShowExceptionsCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowOverloadCount, IdentifierTooltipShowOverloadCountCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseTypeKeywords, IdentifierTooltipUseTypeKeywordsCheckBox, enabledCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations, IdentifierTooltipShowIdentifierAnnotationsComboBox, enabledCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ShowParametersAnnotations, IdentifierTooltipShowParametersAnnotationsComboBox, enabledCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ConstructorReferenceDisplay, IdentifierTooltipConstructorReferenceDisplayComboBox, enabledCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.AttributeConstructorReferenceDisplay, IdentifierTooltipAttributeConstructorReferenceDisplayComboBox, enabledCheckBox);
		}

		private void SetIssueTooltipSettingsBindings() {
			SetCheckBoxBinding((IssueTooltipSettings s) => s.ShowIcon, IssueTooltipShowIconCheckBox, null);
			SetCheckBoxBinding((IssueTooltipSettings s) => s.ColorizeElementsInErrors, IssueTooltipColorizeElementsInErrors, null);
		}

		private void SetParameterInfoSettingsBindings() {
			CheckBoxDisabledNoCheck2 enabledCheckBox = ParameterInfoEnabledCheckBox;
			SetCheckBoxBinding((ParameterInfoSettings s) => s.Enabled, enabledCheckBox, null);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.ShowEmptyParametersText, ParameterInfoShowEmptyParametersTextCheckBox, enabledCheckBox);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.UseTypeKeywords, ParameterInfoUseTypeKeywordsCheckBox, enabledCheckBox);
		}

		private void SetDisplaySettingsBindings() {
			CheckBoxDisabledNoCheck2 enabledCheckBox = DisplayLimitTooltipWidth;
			SetCheckBoxBinding((DisplaySettings s) => s.LimitTooltipWidth, enabledCheckBox, null, addColonToDescription: true);
			SetNumericUpDownBinding((DisplaySettings s) => s.ScreenWidthLimitPercent, DisplayScreenWidthLimitPercent, enabledCheckBox);
		}

		public EnhancedTooltipOptionsPage([NotNull] Lifetime lifetime, [NotNull] OptionsSettingsSmartContext context) {
			_lifetime = lifetime;
			_context = context;

			InitializeComponent();

			SetIdentifierTooltipSettingsBindings();
			SetIssueTooltipSettingsBindings();
			SetParameterInfoSettingsBindings();
			SetDisplaySettingsBindings();
		}

	}

}