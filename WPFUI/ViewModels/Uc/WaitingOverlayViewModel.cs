using ReactiveUI;
using System.Reactive.Linq;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class WaitingOverlayViewModel : ViewModelBase
    {
        public WaitingOverlayViewModel()
        {
            Show("is initializing");
        }

        public void Show(string message)
        {
            Observable.Start(() => BusyMessage = message);
        }

        public void Close()
        {
            Observable.Start(() => BusyMessage = null);
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