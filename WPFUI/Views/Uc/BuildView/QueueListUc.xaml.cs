using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc.BuildView;

namespace WPFUI.Views.Uc.BuildView
{
    /// <summary>
    /// Interaction logic for QueueListUc.xaml
    /// </summary>
    public partial class QueueListUc : ReactiveUserControl<QueueListViewModel>
    {
        public QueueListUc()
        {
            ViewModel = Locator.Current.GetService<QueueListViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Buildings, v => v.Buildings.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.CurrentItem, v => v.Buildings.SelectedItem).DisposeWith(d);
            });
        }
    }
}