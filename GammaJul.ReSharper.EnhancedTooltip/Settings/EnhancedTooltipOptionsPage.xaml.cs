using System;
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
using JetBrains.UI.Options;

namespace GammaJul.ReSharper.EnhancedTooltip.Settings {

	[OptionsPage(Pid, "Enhanced Tooltip", typeof(GenericThemedIcons.Logo16), ParentId = IntelliSensePage.PID, Sequence = 100)]
	public partial class EnhancedTooltipOptionsPage : IOptionsPage {

		public const string Pid = "EnhancedTooltip.OptionsPage";

		private readonly Lifetime _lifetime;
		private readonly OptionsSettingsSmartContext _context;

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

		private void SetCheckBoxBinding<TSettings>([NotNull] Expression<Func<TSettings, bool>> settingAccessor, [NotNull] CheckBoxDisabledNoCheck2 checkBox,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckbox) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			if (checkBox.Content == null)
				checkBox.Content = entry.Description;
			_context.SetBinding<bool>(_lifetime, entry, checkBox, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

			if (parentCheckbox != null)
				parentCheckbox.IsCheckedLogically.FlowInto(_lifetime, checkBox, IsEnabledProperty);
		}

		private void SetComboBoxBinding<TSettings>([NotNull] Expression<Func<TSettings, AnnotationsDisplayKind>> settingAccessor, [NotNull] ComboBox comboBox,
			[CanBeNull] CheckBoxDisabledNoCheck2 parentCheckbox) {

			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			_context.SetBinding<AnnotationsDisplayKind>(_lifetime, entry, comboBox, Selector.SelectedItemProperty);

			if (parentCheckbox != null)
				parentCheckbox.IsCheckedLogically.FlowInto(_lifetime, comboBox, IsEnabledProperty);
		}

		private void SetIdentifierTooltipSettingsBindings() {
			CheckBoxDisabledNoCheck2 enabledCheckBox = IdentifierTooltipEnabledCheckBox;
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.Enabled, enabledCheckBox, null);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowIcon, IdentifierTooltipShowIconCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowKind, IdentifierTooltipShowKindCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowObsolete, IdentifierTooltipShowObsoleteCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowExceptions, IdentifierTooltipShowExceptionsCheckBox, enabledCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseTypeKeywords, IdentifierTooltipUseTypeKeywordsCheckBox, enabledCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ShowIdentifierAnnotations, IdentifierTooltipShowIdentifierAnnotationsComboBox, enabledCheckBox);
			SetComboBoxBinding((IdentifierTooltipSettings s) => s.ShowParametersAnnotations, IdentifierTooltipShowParametersAnnotationsComboBox, enabledCheckBox);
		}

		private void SetIssueTooltipSettingsBindings() {
			SetCheckBoxBinding((IssueTooltipSettings s) => s.ShowIcon, IssueTooltipShowIconCheckBox, null);
		}

		private void SetParameterInfoSettingsBindings() {
			CheckBoxDisabledNoCheck2 enabledCheckBox = ParameterInfoEnabledCheckBox;
			SetCheckBoxBinding((ParameterInfoSettings s) => s.Enabled, enabledCheckBox, null);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.ShowEmptyParametersText, ParameterInfoShowEmptyParametersTextCheckBox, enabledCheckBox);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.UseTypeKeywords, ParameterInfoUseTypeKeywordsCheckBox, enabledCheckBox);
#if RS90
			// ReSharper 9 already has this option built-in
			ParameterInfoShowAnnotationsComboBox.Visibility = System.Windows.Visibility.Collapsed;
#elif RS82
			SetComboBoxBinding((ParameterInfoSettings s) => s.ShowAnnotations, ParameterInfoShowAnnotationsComboBox, enabledCheckBox);
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