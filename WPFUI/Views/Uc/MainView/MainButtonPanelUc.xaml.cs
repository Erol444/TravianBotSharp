using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Views.Uc.MainView
{
    public class MainButtonPanelUcBase : ReactiveUserControl<MainButtonPanelViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainButtonPanelUc.xaml
    /// </summary>
    public partial class MainButtonPanelUc : MainButtonPanelUcBase
    {
        public MainButtonPanelUc()
        {
            ViewModel = Locator.Current.GetService<MainButtonPanelViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.CheckVersionCommand, v => v.CheckVersionButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountCommand, v => v.AddAccountButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddAccountsCommand, v => v.AddAccountsButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAccountCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PauseCommand, v => v.PauseButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TextPause, v => v.PauseButton.Content).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RestartCommand, v => v.RestartButton).DisposeWith(d);
            });
        }
    }
}