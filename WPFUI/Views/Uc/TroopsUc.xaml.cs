using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for TroopsUc.xaml
    /// </summary>
    public partial class TroopsUc : ReactiveUserControl<TroopsViewModel>
    {
        public TroopsUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsReadOnly, v => v.Troops.IsEnabled).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Troops, v => v.Troops.ItemsSource).DisposeWith(d);
            });
        }
    }
}