using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    public class VersionOverlayUcBase : ReactiveUserControl<VersionOverlayViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VersionOverlayUc.xaml
    /// </summary>
    public partial class VersionOverlayUc : VersionOverlayUcBase
    {
        public VersionOverlayUc()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.IsOpen, v => v.Grid.Visibility).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DiscordCommand, v => v.DiscordButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LatestVersionCommand, v => v.LastVersionButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.CloseButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Message, v => v.StatusLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVersion, v => v.CurrentLabel.Content).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.LatestVersion, v => v.LastVersionLabel.Content).DisposeWith(d);
            });
        }
    }
}