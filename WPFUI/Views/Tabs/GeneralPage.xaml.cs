using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for GeneralPage.xaml
    /// </summary>
    public partial class GeneralPage : ReactivePage<GeneralViewModel>
    {
        public GeneralPage()
        {
            ViewModel = new();
            InitializeComponent();

            this.WhenActivated(d =>
            {
                #region command

                this.BindCommand(ViewModel,
                    vm => vm.PauseCommand,
                    v => v.PauseButton)
                .DisposeWith(d);
                this.BindCommand(ViewModel,
                    vm => vm.ResumeCommand,
                    v => v.ResumeButton)
                .DisposeWith(d);
                this.BindCommand(ViewModel,
                    vm => vm.RestartCommand,
                    v => v.RestartButton)
                .DisposeWith(d);

                #endregion command

                #region data

                this.Bind(ViewModel,
                    vm => vm.ClickDelay,
                    v => v.ClickDelayTextBox.Text)
                .DisposeWith(d);
                this.Bind(ViewModel,
                    vm => vm.ClickDelayRange,
                    v => v.ClickDelayRangeTextBox.Text)
                .DisposeWith(d);

                this.Bind(ViewModel,
                   vm => vm.TaskDelay,
                   v => v.TaskDelayTextBox.Text)
               .DisposeWith(d);
                this.Bind(ViewModel,
                    vm => vm.TaskDelayRange,
                    v => v.TaskDelayRangeTextBox.Text)
                .DisposeWith(d);

                this.Bind(ViewModel,
                   vm => vm.WorkTime,
                   v => v.WorkTextBox.Text)
               .DisposeWith(d);
                this.Bind(ViewModel,
                    vm => vm.WorkTimeRange,
                    v => v.WorkRangeTextBox.Text)
                .DisposeWith(d);

                this.Bind(ViewModel,
                   vm => vm.SleepTime,
                   v => v.SleepTextBox.Text)
               .DisposeWith(d);
                this.Bind(ViewModel,
                    vm => vm.SleepTimeRange,
                    v => v.SleepRangeTextBox.Text)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.IsDontLoadImage,
                    v => v.DisableImageCheckBox.IsChecked)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.IsDontLoadImage,
                    v => v.MinimizedCheckBox.IsChecked)
                .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.IsDontLoadImage,
                    v => v.CloseCheckBox.IsChecked)
                .DisposeWith(d);

                #endregion data
            });
        }
    }
}