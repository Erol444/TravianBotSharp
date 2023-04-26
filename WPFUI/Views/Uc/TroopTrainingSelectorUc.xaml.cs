using ReactiveUI;
using System.Windows;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    /// <summary>
    /// Interaction logic for TroopTrainingSelectorUc.xaml
    /// </summary>
    public partial class TroopTrainingSelectorUc : ReactiveUserControl<TroopTrainingSelectorViewModel>
    {
        public TroopTrainingSelectorUc()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(TroopTrainingSelectorUc), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty GreatProperty =
           DependencyProperty.Register("Great", typeof(bool), typeof(TroopTrainingSelectorUc), new PropertyMetadata(default(bool)));

        public bool Great
        {
            get => (bool)GetValue(GreatProperty);
            set => SetValue(GreatProperty, value);
        }
    }
}