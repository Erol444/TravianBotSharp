using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class TroopsPageBase : ReactivePage<VillageTroopsViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for TroopsPage.xaml
    /// </summary>
    public partial class TroopsPage : TroopsPageBase
    {
        public TroopsPage()
        {
            ViewModel = Locator.Current.GetService<VillageTroopsViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ApplyCommand, v => v.Apply).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpdateCommand, v => v.Update).DisposeWith(d);
            });
        }
    }
}