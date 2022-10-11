using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for CheckBoxWithInput.xaml
    /// </summary>
    public partial class CheckBoxWithInputUc : ReactiveUserControl<CheckBoxWithInputViewModel>
    {
        public CheckBoxWithInputUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.IsChecked, v => v.IsChecked.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Text, v => v.IsChecked.Content).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Value, v => v.Value.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Unit, v => v.UnitLabel.Content).DisposeWith(d);
            });
        }
    }
}