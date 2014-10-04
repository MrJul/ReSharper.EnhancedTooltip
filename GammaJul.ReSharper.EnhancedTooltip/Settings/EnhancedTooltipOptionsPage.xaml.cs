using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
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

		private void SetCheckBoxBinding<TSettings>([NotNull] Expression<Func<TSettings, bool>> settingAccessor, [NotNull] CheckBoxDisabledNoCheck2 checkBox) {
			SettingsScalarEntry entry = _context.Schema.GetScalarEntry(settingAccessor);
			checkBox.Content = entry.Description;
			_context.SetBinding<bool>(_lifetime, entry, checkBox, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
		}

		private void SetIdentifierTooltipSettingsBindings() {
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowIcon, IdentifierTooltipShowIconCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowKind, IdentifierTooltipShowKindCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowObsolete, IdentifierTooltipShowObsoleteCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowExceptions, IdentifierTooltipShowExceptionsCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.UseTypeKeywords, IdentifierTooltipUseTypeKeywordsCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowIdentifierNullness, IdentifierTooltipShowIdentifierNullnessCheckBox);
			SetCheckBoxBinding((IdentifierTooltipSettings s) => s.ShowParametersNullness, IdentifierTooltipShowParametersNullnessCheckBox);
		}

		private void SetIssueTooltipSettingsBindings() {
			SetCheckBoxBinding((IssueTooltipSettings s) => s.ShowIcon, IssueTooltipShowIconCheckBox);
		}

		private void SetParameterInfoSettingsBindings() {
			SetCheckBoxBinding((ParameterInfoSettings s) => s.UseTypeKeywords, ParameterInfoUseTypeKeywordsCheckBox);
			SetCheckBoxBinding((ParameterInfoSettings s) => s.ShowParametersNullness, ParameterInfoShowParametersNullnessCheckBox);
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