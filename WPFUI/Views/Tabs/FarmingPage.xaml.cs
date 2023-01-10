using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for FarmingPage.xaml
    /// </summary>
    public partial class FarmingPage : ReactivePage<FarmingViewModel>
    {
        public FarmingPage()
        {
            ViewModel = Locator.Current.GetService<FarmingViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.StartCommand, v => v.StartButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StopCommand, v => v.StopButton).DisposeWith(d);
            });
        }
    }
}