using ReactiveUI;
using Splat;
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
            ViewModel = Locator.Current.GetService<SettingsViewModel>();
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Tribes, v => v.Tribe.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedTribe, v => v.Tribe.SelectedItem).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.ClickDelay, v => v.ClickDelay.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TaskDelay, v => v.TaskDelay.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WorkTime, v => v.WorkTime.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SleepTime, v => v.SleepTime.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsSleepBetweenProxyChanging, v => v.SleepBetweenChangingProxy.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsDontLoadImage, v => v.DisableImageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsMinimized, v => v.MinimizedCheckBox.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsAutoStartAdventure, v => v.AutoStartAdventureCheckBox.IsChecked).DisposeWith(d);
            });
        }
    }
}