using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class InfoTabBase : ReactiveUserControl<InfoViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for InfoPage.xaml
    /// </summary>
    public partial class InfoTab : InfoTabBase
    {
        public InfoTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.BothDorfCommand, v => v.UpdateDorfButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Dorf1Command, v => v.UpdateDorf1Button).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Dorf2Command, v => v.UpdateDorf2Button).DisposeWith(d);
            });
        }
    }
}