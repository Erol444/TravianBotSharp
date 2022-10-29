using Avalonia.ReactiveUI;
using ReactiveUI;
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
                this.Bind(ViewModel, vm => vm.IsLoading, v => v.Overlay.IsVisible);
            });
        }
    }
}