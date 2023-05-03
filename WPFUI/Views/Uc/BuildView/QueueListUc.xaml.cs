using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.BuildView;

namespace WPFUI.Views.Uc.BuildView
{
    public class QueueListUcBase : ReactiveUserControl<QueueListViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for QueueListUc.xaml
    /// </summary>
    public partial class QueueListUc : QueueListUcBase
    {
        public QueueListUc()
        {
            ViewModel = Locator.Current.GetService<QueueListViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.TopCommand, v => v.TopButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.BottomCommand, v => v.BottomButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpCommand, v => v.UpButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DownCommand, v => v.DownButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.DeleteButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteAllCommand, v => v.DeleteAllButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentItem, v => v.Buildings.SelectedItem).DisposeWith(d);
            });
        }
    }
}