using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for ToleranceUc.xaml
    /// </summary>
    public partial class ToleranceUc : ReactiveUserControl<ToleranceViewModel>
    {
        public ToleranceUc()

        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.MainValue, v => v.MainValue.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ToleranceValue, v => v.Tolerance.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ToleranceMax, v => v.Tolerance.Maximum).DisposeWith(d);
            });
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ToleranceUc), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ToleranceUc), new PropertyMetadata(default(string)));

        public string Unit
        {
            get => (string)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }
    }
}