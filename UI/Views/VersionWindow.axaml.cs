using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels;

namespace UI.Views
{
    public partial class VersionWindow : ReactiveWindow<VersionViewModel>
    {
        public VersionWindow()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<VersionViewModel>();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.DiscordCommand, v => v.DiscordButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LatestVersionCommand, v => v.LastVersionButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Message, v => v.StatusLabel.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentVersion, v => v.CurrentLabel.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.LatestVersion, v => v.LastVersionLabel.Text).DisposeWith(d);
            });
        }
    }
}