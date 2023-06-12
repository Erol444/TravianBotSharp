using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class TroopsTabBase : ReactiveUserControl<VillageTroopsViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for TroopsPage.xaml
    /// </summary>
    public partial class TroopsTab : TroopsTabBase
    {
        public TroopsTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ApplyCommand, v => v.Apply).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpdateCommand, v => v.Update).DisposeWith(d);
            });
        }
    }
}