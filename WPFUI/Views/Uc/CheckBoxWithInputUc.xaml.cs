using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
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
                this.Bind(ViewModel, vm => vm.Value, v => v.Value.Value).DisposeWith(d);
            });
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CheckBoxWithInputUc), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(CheckBoxWithInputUc), new PropertyMetadata(default(string)));

        public string Unit
        {
            get => (string)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }
    }
}