using ReactiveUI;
using System.Collections.ObjectModel;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc
{
    public class TroopsViewModel : ReactiveObject
    {
        public TroopsViewModel(string text, bool isReadOnly = false) : base()
        {
            Text = text;
            IsReadOnly = isReadOnly;
        }

        private bool _isReadOnly;

        public bool IsReadOnly
        {
            get => !_isReadOnly;
            set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private ObservableCollection<TroopInfo> _troops;

        public ObservableCollection<TroopInfo> Troops
        {
            get => _troops;
            set => this.RaiseAndSetIfChanged(ref _troops, value);
        }
    }
}