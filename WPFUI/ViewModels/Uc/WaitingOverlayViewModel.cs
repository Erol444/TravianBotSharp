using ReactiveUI;
using System.Reactive.Concurrency;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class WaitingOverlayViewModel : ViewModelBase
    {
        public WaitingOverlayViewModel()
        {
            BusyMessage = "is initializing ...";
        }

        public void Show(string message)
        {
            RxApp.MainThreadScheduler.Schedule(() => BusyMessage = message);
        }

        public void Close()
        {
            RxApp.MainThreadScheduler.Schedule(() => BusyMessage = null);
        }

        private string _busyMessage;

        public string BusyMessage
        {
            get => _busyMessage;
            set
            {
                var formattedValue = string.IsNullOrWhiteSpace(value) ? value : $"TBS is {value} ...";
                this.RaiseAndSetIfChanged(ref _busyMessage, formattedValue);
            }
        }
    }
}