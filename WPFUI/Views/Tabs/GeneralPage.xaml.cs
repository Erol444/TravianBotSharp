using ReactiveUI;
using Splat;
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
            ViewModel = Locator.Current.GetService<GeneralViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.PauseCommand, v => v.PauseButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RestartCommand, v => v.RestartButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Status, v => v.StatusText.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.PauseText, v => v.PauseButton.Content).DisposeWith(d);
            });
        }
    }
}