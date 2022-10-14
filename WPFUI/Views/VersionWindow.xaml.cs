using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for VersionWindow.xaml
    /// </summary>
    public partial class VersionWindow : ReactiveWindow<VersionViewModel>
    {
        public VersionWindow()
        {
            ViewModel = new();
            ViewModel.CloseWindow += Hide;
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.DiscordCommand, v => v.DiscordButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LatestVersionCommand, v => v.LastVersionButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LatestBuildCommand, v => v.LastBuildButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.CloseButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Message, v => v.StatusLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVersion, v => v.CurrentLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.LatestVersion, v => v.LastVersionLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.LatestBuild, v => v.LastBuildLabel.Text).DisposeWith(d);
            });
        }
    }
}