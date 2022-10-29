using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class AccountTableUc : ReactiveUserControl<AccountTableViewModel>
    {
        public AccountTableUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Accounts, v => v.Accounts.Items).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentAccount, v => v.Accounts.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentIndex, v => v.Accounts.SelectedIndex).DisposeWith(d);
            });
        }
    }
}