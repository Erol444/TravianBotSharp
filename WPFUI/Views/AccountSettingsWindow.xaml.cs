using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for AccountSettings.xaml
    /// </summary>
    public partial class AccountSettingsWindow : ReactiveWindow<AccountSettingsViewModel>
    {
        public AccountSettingsWindow()
        {
            ViewModel = new();
            ViewModel.CloseView += Hide;
            InitializeComponent();

            #region command

            this.BindCommand(ViewModel,
                vm => vm.SaveCommand,
                v => v.SaveButton);

            this.BindCommand(ViewModel,
                vm => vm.CloseCommand,
                v => v.CloseButton);

            this.BindCommand(ViewModel,
                vm => vm.ImportCommand,
                v => v.ImportButton);

            this.BindCommand(ViewModel,
                vm => vm.ExportCommand,
                v => v.ExportButton);

            #endregion command

            #region data

            this.Bind(ViewModel,
                vm => vm.ClickDelay,
                v => v.ClickDelayTextBox.Text);

            this.Bind(ViewModel,
                vm => vm.ClickDelayRange,
                v => v.ClickDelayRangeTextBox.Text);

            this.Bind(ViewModel,
               vm => vm.TaskDelay,
               v => v.TaskDelayTextBox.Text);
            this.Bind(ViewModel,
                vm => vm.TaskDelayRange,
                v => v.TaskDelayRangeTextBox.Text);

            this.Bind(ViewModel,
               vm => vm.WorkTime,
               v => v.WorkTextBox.Text);
            this.Bind(ViewModel,
                vm => vm.WorkTimeRange,
                v => v.WorkRangeTextBox.Text);

            this.Bind(ViewModel,
               vm => vm.SleepTime,
               v => v.SleepTextBox.Text);
            this.Bind(ViewModel,
                vm => vm.SleepTimeRange,
                v => v.SleepRangeTextBox.Text);

            this.Bind(ViewModel,
                vm => vm.IsDontLoadImage,
                v => v.DisableImageCheckBox.IsChecked);

            this.Bind(ViewModel,
                vm => vm.IsMinimized,
                v => v.MinimizedCheckBox.IsChecked);

            this.Bind(ViewModel,
                vm => vm.IsClosedIfNoTask,
                v => v.CloseCheckBox.IsChecked);

            this.Bind(ViewModel,
                vm => vm.IsAutoStartAdventure,
                v => v.AutoStartAdventureCheckBox.IsChecked);

            this.Bind(ViewModel,
                vm => vm.Username,
                v => v.UsernameLabel.Text);

            this.Bind(ViewModel,
                vm => vm.Server,
                v => v.ServerLabel.Text);

            #endregion data
        }
    }
}