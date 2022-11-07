using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.Tabs;
using UI.ViewModels.UserControls;

namespace UI.Views.Tabs
{
    public partial class SettingsTab : ReactiveUserControl<SettingsViewModel>
    {
        public SettingsTab()
        {
            InitializeComponent();

            ViewModel = Locator.Current.GetService<SettingsViewModel>();

            var clickDelayViewModel = Locator.Current.GetService<ToleranceViewModel>();
            clickDelayViewModel.Text = "Click delay";
            clickDelayViewModel.Unit = "ms";
            ClickDelay.ViewModel = clickDelayViewModel;

            var taskDelayViewModel = Locator.Current.GetService<ToleranceViewModel>();
            taskDelayViewModel.Text = "Task delay";
            taskDelayViewModel.Unit = "ms";
            TaskDelay.ViewModel = taskDelayViewModel;

            var workTimeViewModel = Locator.Current.GetService<ToleranceViewModel>();
            workTimeViewModel.Text = "Work time";
            workTimeViewModel.Unit = "mins";
            WorkTime.ViewModel = workTimeViewModel;

            var sleepTimeViewModel = Locator.Current.GetService<ToleranceViewModel>();
            sleepTimeViewModel.Text = "Sleep time";
            sleepTimeViewModel.Unit = "mins";
            SleepTime.ViewModel = sleepTimeViewModel;

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.ClickDelay, v => v.ClickDelay.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.ClickDelayRange, v => v.ClickDelay.ViewModel.Tolerance).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.TaskDelay, v => v.TaskDelay.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.TaskDelayRange, v => v.TaskDelay.ViewModel.Tolerance).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.WorkTime, v => v.WorkTime.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.WorkTimeRange, v => v.WorkTime.ViewModel.Tolerance).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SleepTime, v => v.SleepTime.ViewModel.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SleepTimeRange, v => v.SleepTime.ViewModel.Tolerance).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsDontLoadImage, v => v.DisableImageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsMinimized, v => v.MinimizedCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.IsClosedIfNoTask, v => v.CloseCheckBox.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsAutoStartAdventure, v => v.AutoStartAdventureCheckBox.IsChecked).DisposeWith(d);
            });
        }
    }
}