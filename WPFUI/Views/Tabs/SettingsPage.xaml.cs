using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : ReactivePage<SettingsViewModel>
    {
        public SettingsPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.ClickDelay, v => v.ClickDelayTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.ClickDelayRange, v => v.ClickDelayRangeTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.TaskDelay, v => v.TaskDelayTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.TaskDelayRange, v => v.TaskDelayRangeTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.WorkTime, v => v.WorkTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.WorkTimeRange, v => v.WorkRangeTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SleepTime, v => v.SleepTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SleepTimeRange, v => v.SleepRangeTextBox.Text).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsDontLoadImage, v => v.DisableImageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsMinimized, v => v.MinimizedCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsClosedIfNoTask, v => v.CloseCheckBox.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsAutoStartAdventure, v => v.AutoStartAdventureCheckBox.IsChecked).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}