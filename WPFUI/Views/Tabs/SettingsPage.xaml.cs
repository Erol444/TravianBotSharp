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

            ClickDelay.ViewModel = new("Click delay", "ms");
            TaskDelay.ViewModel = new("Task delay", "ms");
            WorkTime.ViewModel = new("Work time", "mins");
            SleepTime.ViewModel = new("Sleep time", "mins");

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.ClickDelay, v => v.ClickDelay.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.ClickDelayRange, v => v.ClickDelay.ViewModel.ToleranceValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.TaskDelay, v => v.TaskDelay.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.TaskDelayRange, v => v.TaskDelay.ViewModel.ToleranceValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.WorkTime, v => v.WorkTime.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.WorkTimeRange, v => v.WorkTime.ViewModel.ToleranceValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SleepTime, v => v.SleepTime.ViewModel.MainValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SleepTimeRange, v => v.SleepTime.ViewModel.ToleranceValue).DisposeWith(d);

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