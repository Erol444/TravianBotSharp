using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for EditAccountPage.xaml
    /// </summary>
    public partial class EditAccountPage : ReactivePage<EditAccountViewModel>
    {
        public EditAccountPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.TestAllCommand, v => v.TestAllProxiesButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Server, v => v.ServerTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Username, v => v.UsernameTextBox.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Accessess, v => v.ProxiesDataGrid.ItemsSource).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}