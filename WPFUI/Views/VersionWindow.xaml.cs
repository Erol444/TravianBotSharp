using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    public class VersionWindowBase : ReactiveWindow<VersionViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for VersionWindow.xaml
    /// </summary>
    public partial class VersionWindow : VersionWindowBase
    {
        public VersionWindow()
        {
            ViewModel = Locator.Current.GetService<VersionViewModel>();
            ViewModel.Close = Hide;
            ViewModel.Show = Show;
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.DiscordCommand, v => v.DiscordButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LatestVersionCommand, v => v.LastVersionButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Message, v => v.StatusLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentVersion, v => v.CurrentLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.LatestVersion, v => v.LastVersionLabel.Text).DisposeWith(d);
            });
        }
    }
}