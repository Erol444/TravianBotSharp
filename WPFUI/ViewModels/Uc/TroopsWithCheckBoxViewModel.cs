using ReactiveUI;
using System.Collections.ObjectModel;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc
{
    public class TroopsWithCheckBoxViewModel : ReactiveObject
    {
        public TroopsWithCheckBoxViewModel(string text) : base()
        {
            Text = text;
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private ObservableCollection<TroopInfoCheckBox> _troops;

        public ObservableCollection<TroopInfoCheckBox> Troops
        {
            get => _troops;
            set => this.RaiseAndSetIfChanged(ref _troops, value);
        }
    }
}