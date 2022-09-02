using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for AccountTableUc.xaml
    /// </summary>
    public partial class AccountTableUc : ReactiveUserControl<AccountTableViewModel>
    {
        public AccountTableUc()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Accounts,
                    v => v.AccountGrid.ItemsSource)
                .DisposeWith(d);
                this.Bind(ViewModel,
                    vm => vm.CurrentAccount,
                    v => v.AccountGrid.CurrentItem)
                .DisposeWith(d);

                ViewModel.OnActived();
            });
        }
    }
}