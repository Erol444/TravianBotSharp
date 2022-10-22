using ReactiveUI;
using System.Collections.ObjectModel;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc
{
    public class TroopsViewModel : ReactiveObject
    {
        public TroopsViewModel(string text) : base()
        {
            Text = text;
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private ObservableCollection<TroopInfoText> _troops;

        public ObservableCollection<TroopInfoText> Troops
        {
            get => _troops;
            set => this.RaiseAndSetIfChanged(ref _troops, value);
        }
    }
}