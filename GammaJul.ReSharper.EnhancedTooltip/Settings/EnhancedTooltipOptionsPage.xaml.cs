using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Features.Intellisense.Options;
using JetBrains.UI.Controls;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Extensions;
using JetBrains.UI.Options;
using JetBrains.UI.Wpf.Converters;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[OptionsPage(Pid, "Enhanced Tooltip", typeof(ThemedIcons.Logo16), ParentId = IntelliSensePage.PID, Sequence = 100)]
	public partial class EnhancedTooltipOptionsPage : IOptionsPage {

		public const string Pid = "EnhancedTooltip.OptionsPage";

		[NotNull] private readonly List<EnumValue> _annotationsItemsSource = CreateAnnotationsItemsSource();
		[NotNull] private readonly Lifetime _lifetime;
		[NotNull] private readonly OptionsSettingsSmartContext _context;

		public string Id {
			get { return Pid; }
		}

		public bool OnOk() {
			return true;
		}

		public bool ValidatePage() {
			return true;
		}

		public EitherControl Control {
			get { return this; }
		}

		[NotNull]
		private static List<EnumValue> CreateAnnotationsItemsSource() {
			var enumDescriptionConverter = new EnumDescriptionConverter();
			return Enum.GetValues(typeof(AnnotationsDisplayKind))
				.Cast<Enum>()
				.Select(e => new EnumValue(e, enumDescriptionConverter.Convert(e, typeof(string), null, CultureInfo.CurrentUICulture) as string))
				.ToList();
		}

		private void SetCheckBoxBinding<TSettings>([NotNull] Expression<Func<TSettings, bool>> settingAccessor, [NotNull] CheckBoxDisabledNoCheck2 checkBox,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckbox) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			if (checkBox.Content == null)
				checkBox.Content = entry.Description;
			_context.SetBinding<bool>(_lifetime, entry, checkBox, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

			if (parentCheckbox != null)
				parentCheckbox.IsCheckedLogically.FlowInto(_lifetime, checkBox, IsEnabledProperty);
		}

		private void SetAnnotationsBinding<TSettings>([NotNull] Expression<Func<TSettings, AnnotationsDisplayKind>> settingAccessor, [NotNull] ComboBox comboBox,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckbox) {

			comboBox.ItemsSource = _annotationsItemsSource;
			comboBox.DisplayMemberPath = "Description";
			comboBox.SelectedValuePath = "Value";

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			_context.SetBinding<object>(_lifetime, entry, comboBox, Selector.SelectedValueProperty);
			
			Label label = FindAssociatedLabel(comboBox);
			if (label != null && label.Content == null)
				label.Content = entry.Description + ":";
			
			if (parentCheckbox != null)
				parentCheckbox.IsCheckedLogically.FlowInto(_lifetime, comboBox, IsEnabledProperty);
		}

		[CanBeNull]
		private static Label FindAssociatedLabel([NotNull] ComboBox comboBox) {
			return comboBox.Parent != null ? comboBox.Parent.FindDescendant<Label>() : null;
		}

		private void SetIdentifierTooltipSettingsBindings() {
			CheckBoxDisabledNoCheck2 enabledCheckBox = IdentifierTooltipEnabledCheckBox;
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.Enabled, enabledCheckBox, null);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowIcon, IdentifierTooltipShowIconCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowKind, IdentifierTooltipShowKindCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowObsolete, IdentifierTooltipShowObsoleteCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowExceptions, IdentifierTooltipShowExceptionsCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseTypeKeywords, IdentifierTooltipUseTypeKeywordsCheckBox, enabledCheckBox);
			SetAnnotationsBinding((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations, IdentifierTooltipShowIdentifierAnnotationsComboBox, enabledCheckBox);
			SetAnnotationsBinding((IdentifierTooltipSettings s) => s.ShowParametersAnnotations, IdentifierTooltipShowParametersAnnotationsComboBox, enabledCheckBox);
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
#if RS90
			// ReSharper 9 already has this option built-in
			ParameterInfoShowAnnotationsPanel.Visibility = System.Windows.Visibility.Collapsed;
#elif RS82
			SetAnnotationsBinding((ParameterInfoSettings s) => s.ShowAnnotations, ParameterInfoShowAnnotationsComboBox, enabledCheckBox);
#endif
		}

		public EnhancedTooltipOptionsPage([NotNull] Lifetime lifetime, [NotNull] OptionsSettingsSmartContext context) {
			_lifetime = lifetime;
			_context = context;

			InitializeComponent();

			SetIdentifierTooltipSettingsBindings();
			SetIssueTooltipSettingsBindings();
			SetParameterInfoSettingsBindings();
		}

	}

}