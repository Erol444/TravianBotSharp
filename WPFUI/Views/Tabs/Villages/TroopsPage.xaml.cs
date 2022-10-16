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
            CurrentLevel.ViewModel = new("Current troops's level: ", true);
            WantLevel.ViewModel = new("I want to this level: ");
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ApplyCommand, v => v.Apply).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CurrentLevel, v => v.CurrentLevel.ViewModel.Troops).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.WantLevel, v => v.WantLevel.ViewModel.Troops).DisposeWith(d);
                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}