using ReactiveUI;
using System.Reactive;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class WaitingOverlayViewModel : ViewModelBase
    {
        public ReactiveCommand<string, Unit> ShowCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public WaitingOverlayViewModel()
        {
            ShowCommand = ReactiveCommand.Create<string>(ShowTask);
            CloseCommand = ReactiveCommand.Create(CloseTask);
            BusyMessage = "is initializing ...";
        }

        private void ShowTask(string message)
        {
            BusyMessage = message;
        }

        private void CloseTask()
        {
            BusyMessage = null;
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