using ReactiveUI;

namespace WPFUI.ViewModels
{
    public class WaitingViewModel : ReactiveObject
    {
        private string _text;

        public string Text
        {
            get => $"TBS is {_text} . . .";
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }
}