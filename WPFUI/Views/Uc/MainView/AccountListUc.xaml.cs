using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Views.Uc.MainView
{
    /// <summary>
    /// Interaction logic for AccountListUc.xaml
    /// </summary>
    public partial class AccountListUc : ReactiveUserControl<AccountListViewModel>
    {
        public AccountListUc()
        {
            ViewModel = Locator.Current.GetService<AccountListViewModel>();
            InitializeComponent();
            this.WhenActivated((d) =>
            {
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.AccountGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentAccount, v => v.AccountGrid.SelectedItem).DisposeWith(d);
            });
        }
    }
}