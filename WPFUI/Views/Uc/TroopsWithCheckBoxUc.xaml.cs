using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for TroopsWithCheckBoxUc.xaml
    /// </summary>
    public partial class TroopsWithCheckBoxUc : ReactiveUserControl<TroopsWithCheckBoxViewModel>
    {
        public TroopsWithCheckBoxUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v.Text.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Troops, v => v.Troops.ItemsSource).DisposeWith(d);
            });
        }
    }
}