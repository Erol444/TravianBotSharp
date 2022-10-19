using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for TroopsPage.xaml
    /// </summary>
    public partial class TroopsPage : ReactivePage<TroopsViewModel>
    {
        public TroopsPage()
        {
            ViewModel = new();
            InitializeComponent();
            CurrentLevel.ViewModel = new("Current troops's level: ");
            WantUpgrade.ViewModel = new("Select troop for upgrading: ");
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ApplyCommand, v => v.Apply).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpdateCommand, v => v.Update).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentLevel, v => v.CurrentLevel.ViewModel.Troops).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.WantUpgrade, v => v.WantUpgrade.ViewModel.Troops).DisposeWith(d);
                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}