using ReactiveUI;
using System;
using System.Reactive.Concurrency;

namespace WPFUI.ViewModels
{
    public class WaitingViewModel : ReactiveObject
    {
        public Action ShowWindow;

        public Action CloseWindow;

        public void Show(string message)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Text = message;
                ShowWindow();
            });
        }

        public void Close()
        {
            RxApp.MainThreadScheduler.Schedule(CloseWindow);
        }

        private string _text;

        public string Text
        {
            get => $"TBS is {_text} . . .";
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }
}