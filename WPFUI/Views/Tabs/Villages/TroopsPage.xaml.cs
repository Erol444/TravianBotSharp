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
            Level.ViewModel = new("Troops's level: ", true);
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Level, v => v.Level.ViewModel.Troops).DisposeWith(d);
                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}