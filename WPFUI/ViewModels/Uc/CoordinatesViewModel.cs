using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class CoordinatesViewModel : ReactiveObject
    {
        public CoordinatesViewModel(string text) : base()
        {
            Text = text;
        }
        private string _text;

        public string Text
        {
            get => $"{_text}: ";
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }


        private string _xCoordinate;

        public string XCoordinate
        {
            get => _xCoordinate;
            set => this.RaiseAndSetIfChanged(ref _xCoordinate, value);
        }

        private string _yCoordinate;

        public string YCoordinate
        {
            get => _yCoordinate;
            set => this.RaiseAndSetIfChanged(ref _yCoordinate, value);
        }
    }
}