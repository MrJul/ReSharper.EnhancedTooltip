using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Features.Intellisense.Options;
using JetBrains.UI.Controls;
using JetBrains.UI.DataFlow;
using JetBrains.UI.Extensions;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;
using JetBrains.UI.Wpf.Controls.NumericUpDown.Impl;
using JetBrains.UI.Wpf.Converters;
using Lifetime = JetBrains.Lifetimes.Lifetime;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[OptionsPage(Pid, "Enhanced Tooltip", typeof(ThemedIcons.Logo16), ParentId = IntelliSensePage.PID, Sequence = 100)]
	public partial class EnhancedTooltipOptionsPage : IOptionsPage, ISearchablePage {

		public const string Pid = "EnhancedTooltip.OptionsPage";

		private readonly Lifetime _lifetime;
		private readonly OptionsSettingsSmartContext _context;
		private readonly List<OptionsPageKeyword> _keywords = new();

		public event PropertyChangedEventHandler PropertyChanged {
			add { }
			remove { }
		}

		public string Id
			=> Pid;

		public bool OnOk()
			=> true;

		public IProperty<OptionsFilterResult> SearchFilter { get; }
			= new Property<OptionsFilterResult>(String.Format(CultureInfo.InvariantCulture, "Filter for {0}", typeof(EnhancedTooltipOptionsPage)));

		public OptionsPageKeywords GetKeywords()
			=> new OptionsPageKeywords(_keywords);

		public void HighLightKeyword(OptionsFilterResult text)
			=> SearchFilter.Value = text;

		public IEnumerable<string> GetTagKeywordsForPage()
			=> new[] { "Tooltip" };

		private static List<EnumValue> CreateEnumItemsSource(Type enumType) {
			var enumDescriptionConverter = new EnumDescriptionConverter();
			return Enum.GetValues(enumType)
				.Cast<Enum>()
				.Select(e => new EnumValue(e, enumDescriptionConverter.Convert(e, typeof(string), null, CultureInfo.CurrentUICulture) as string ?? e.ToString()))
				.ToList();
		}

		private void SetCheckBoxBinding<TSettings>(
			Expression<Func<TSettings, bool>> settingAccessor,
			CheckBoxDisabledNoCheck2 checkBox,
			CheckBoxDisabledNoCheck2? parentCheckBox,
			bool addColonToDescription = false) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			string description = entry.Description;
			if (addColonToDescription)
				description += ":";
			ConfigureCheckBox(checkBox, description, entry);

			parentCheckBox?.IsAppearingChecked.FlowInto(_lifetime, checkBox, IsEnabledProperty);
		}

		private void SetAttributesArgumentsCheckBoxBinding<TSettings>(
			Expression<Func<TSettings, bool>> settingAccessor,
			CheckBoxDisabledNoCheck2 checkBox,
			ComboBox parentComboBox) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			ConfigureCheckBox(checkBox, "with arguments", entry);

			IProperty<object> selectedValueProperty = DependencyPropertyWrapper.Create<object>(_lifetime, parentComboBox, Selector.SelectedValueProperty, true);
			selectedValueProperty.FlowInto(_lifetime, checkBox, IsEnabledProperty, value => HasAttributesArguments((AttributesDisplayKind) value));
		}

		private void ConfigureCheckBox(CheckBoxDisabledNoCheck2 checkBox, string content, SettingsScalarEntry entry) {
			checkBox.Content ??= content;
			_keywords.Add(new OptionsPageKeyword(content));
			_context.SetBinding<bool>(_lifetime, entry, checkBox.IsCheckedLogically);
			SearchablePageBehavior.SetSearchFilter(checkBox, true);
		}

		[Pure]
		private static bool HasAttributesArguments(AttributesDisplayKind attributesDisplayKind) {
			switch (attributesDisplayKind) {
				case AttributesDisplayKind.AllAnnotations:
				case AttributesDisplayKind.Always:
					return true;
				default:
					return false;
			}
		}

		private void SetComboBoxBinding<TSettings, TEnum>(
			Expression<Func<TSettings, TEnum>> settingAccessor,
			ComboBox comboBox,
			CheckBoxDisabledNoCheck2? parentCheckBox) {

			comboBox.ItemsSource = CreateEnumItemsSource(typeof(TEnum));
			comboBox.DisplayMemberPath = nameof(EnumValue.Description);
			comboBox.SelectedValuePath = nameof(EnumValue.Value);

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
      var selectedValueProperty = DependencyPropertyWrapper.Create<object>(_lifetime, comboBox, Selector.SelectedValueProperty, true);
			_context.SetBinding<object>(_lifetime, entry, selectedValueProperty);

			SetAssociatedLabel(comboBox, entry.Description);

			parentCheckBox?.IsAppearingChecked.FlowInto(_lifetime, comboBox, IsEnabledProperty);
		}

		private void SetNumericUpDownBinding<TSettings>(
			Expression<Func<TSettings, int>> settingAccessor,
			NumericUpDown numericUpDown,
			CheckBoxDisabledNoCheck2? parentCheckBox) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
      var prop = DependencyPropertyWrapper.Create<int>(_lifetime, numericUpDown, NumericUpDown.ValueProperty, true);
			_context.SetBinding<int>(_lifetime, entry,  prop);
			
			SetAssociatedLabel(numericUpDown, entry.Description);

			parentCheckBox?.IsAppearingChecked.FlowInto(_lifetime, numericUpDown, IsEnabledProperty);
		}

		private void SetAssociatedLabel(FrameworkElement element, string description) {
			if (element.Parent?.FindDescendant<Label>() is not { } label)
				return;

			switch (label.Content) {
				case null:
					label.Content = description + ":";
					_keywords.Add(new OptionsPageKeyword(description));
					break;
				case string existingContentString:
					_keywords.Add(new OptionsPageKeyword(existingContentString));
					break;
			}

			SearchablePageBehavior.SetSearchFilter(label, true);
		}

		private void SetIdentifierTooltipSettingsBindings() {
			CheckBoxDisabledNoCheck2 rootCheckBox = IdentifierTooltipEnabledCheckBox;

			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.Enabled, rootCheckBox, null);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowIcon, IdentifierTooltipShowIconCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowAccessRights, IdentifierTooltipShowAccessRightsCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowAccessors, IdentifierTooltipShowAccessorsCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowKind, IdentifierTooltipShowKindCheckBox, rootCheckBox);

			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseExtensionMethodKind, IdentifierTooltipUseExtensionMethodKindCheckBox, IdentifierTooltipShowKindCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseAttributeClassKind, IdentifierTooltipUseAttributeClassKindCheckBox, IdentifierTooltipShowKindCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseMethodModifiersInKind, IdentifierTooltipUseMethodModifiersInKindCheckBox, IdentifierTooltipShowKindCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseClassModifiersInKind, IdentifierTooltipUseClassModifiersInKindCheckBox, IdentifierTooltipShowKindCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseStructModifiersInKind, IdentifierTooltipUseStructModifiersInKindCheckBox, IdentifierTooltipShowKindCheckBox);

			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowDocumentation, IdentifierTooltipShowDocumentationCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowObsolete, IdentifierTooltipShowObsoleteCheckBox, IdentifierTooltipShowDocumentationCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowReturn, IdentifierTooltipShowReturnCheckBox, IdentifierTooltipShowDocumentationCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowValue, IdentifierTooltipShowValueCheckBox, IdentifierTooltipShowDocumentationCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowRemarks, IdentifierTooltipShowRemarksCheckBox, IdentifierTooltipShowDocumentationCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowExceptions, IdentifierTooltipShowExceptionsCheckBox, IdentifierTooltipShowDocumentationCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowParams, IdentifierTooltipShowParametersCheckBox, IdentifierTooltipShowDocumentationCheckBox);

			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowOverloadCount, IdentifierTooltipShowOverloadCountCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowArgumentsRole, IdentifierTooltipShowArgumentsRoleCheckBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.BaseTypeDisplayKind, IdentifierTooltipBaseTypeDisplayKindComboBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ImplementedInterfacesDisplayKind, IdentifierTooltipImplementedInterfacesDisplayKindComboBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowAttributesUsage, IdentifierTooltipShowShowAttributesUsageCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseTypeKeywords, IdentifierTooltipUseTypeKeywordsCheckBox, rootCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseShortNullableForm, IdentifierTooltipUseShortNullableFormCheckBox, rootCheckBox);

			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ShowIdentifierAttributes, IdentifierTooltipShowIdentifierAttributesComboBox, rootCheckBox);
			SetAttributesArgumentsCheckBoxBinding(
				(IdentifierTooltipSettings s) => s.ShowIdentifierAttributesArguments,
				IdentifierTooltipShowIdentifierAttributesArgumentsCheckBox,
				IdentifierTooltipShowIdentifierAttributesComboBox);

			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ShowParametersAttributes, IdentifierTooltipShowParametersAttributesComboBox, rootCheckBox);
			SetAttributesArgumentsCheckBoxBinding(
				(IdentifierTooltipSettings s) => s.ShowParametersAttributesArguments,
				IdentifierTooltipShowParametersAttributesArgumentsCheckBox,
				IdentifierTooltipShowParametersAttributesComboBox);

			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ConstructorReferenceDisplay, IdentifierTooltipConstructorReferenceDisplayComboBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.AttributeConstructorReferenceDisplay, IdentifierTooltipAttributeConstructorReferenceDisplayComboBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.SolutionCodeNamespaceDisplayKind, IdentifierTooltipSolutionCodeNamespaceDisplayKindComboBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ExternalCodeNamespaceDisplayKind, IdentifierTooltipExternalCodeNamespaceDisplayKindComboBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ParametersFormattingMode, IdentifierTooltipParametersFormattingModeComboBox, rootCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.AttributesFormattingMode, IdentifierTooltipAttributesFormattingModeComboBox, rootCheckBox);
		}

		private void SetIssueTooltipSettingsBindings() {
			SetCheckBoxBinding((IssueTooltipSettings s) => s.ShowIcon, IssueTooltipShowIconCheckBox, null);
			SetCheckBoxBinding((IssueTooltipSettings s) => s.ColorizeElementsInErrors, IssueTooltipColorizeElementsInErrors, null);
		}

		private void SetParameterInfoSettingsBindings() {
			CheckBoxDisabledNoCheck2 rootCheckBox = ParameterInfoEnabledCheckBox;
			SetCheckBoxBinding((ParameterInfoSettings s) => s.Enabled, rootCheckBox, null);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.ShowEmptyParametersText, ParameterInfoShowEmptyParametersTextCheckBox, rootCheckBox);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.UseTypeKeywords, ParameterInfoUseTypeKeywordsCheckBox, rootCheckBox);
		}

		private void SetDisplaySettingsBindings() {
			CheckBoxDisabledNoCheck2 rootCheckBox = DisplayLimitTooltipWidth;
			SetCheckBoxBinding((DisplaySettings s) => s.LimitTooltipWidth, rootCheckBox, null, addColonToDescription: true);
			SetNumericUpDownBinding((DisplaySettings s) => s.ScreenWidthLimitPercent, DisplayScreenWidthLimitPercent, rootCheckBox);
			SetComboBoxBinding((DisplaySettings s) => s.TextFormattingMode, DisplayTextFormattingMode, null);
			SetComboBoxBinding((DisplaySettings s) => s.TooltipColorSource, DisplayTooltipColorSource, null);
		}

		public EnhancedTooltipOptionsPage(Lifetime lifetime, OptionsSettingsSmartContext context) {
			_lifetime = lifetime;
			_context = context;

			DataContext = this;
			InitializeComponent();

			SetIdentifierTooltipSettingsBindings();
			SetIssueTooltipSettingsBindings();
			SetParameterInfoSettingsBindings();
			SetDisplaySettingsBindings();
		}

	}

}