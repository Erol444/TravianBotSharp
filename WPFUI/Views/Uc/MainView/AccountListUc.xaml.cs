using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Views.Uc.MainView
{
    public class AccountListUcBase : ReactiveUserControl<AccountListViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for AccountListUc.xaml
    /// </summary>
    public partial class AccountListUc : AccountListUcBase
    {
        public AccountListUc()
        {
            InitializeComponent();
            this.WhenActivated((d) =>
            {
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentAccount, v => v.AccountGrid.SelectedItem).DisposeWith(d);
            });
        }
    }
}