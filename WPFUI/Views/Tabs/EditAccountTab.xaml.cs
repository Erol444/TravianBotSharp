using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class EditAccountTabBase : ReactiveUserControl<EditAccountViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for EditAccountPage.xaml
    /// </summary>
    public partial class EditAccountTab : EditAccountTabBase
    {
        public EditAccountTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Server, v => v.ServerTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Username, v => v.UsernameTextBox.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Accessess, v => v.ProxiesDataGrid.ItemsSource).DisposeWith(d);
            });
        }
    }
}