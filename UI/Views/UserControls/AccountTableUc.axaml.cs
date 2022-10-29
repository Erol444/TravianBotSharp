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
            });
        }
    }
}