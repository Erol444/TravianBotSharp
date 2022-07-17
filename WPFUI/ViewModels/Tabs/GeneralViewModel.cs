using ReactiveUI;
using System.Reactive;

namespace WPFUI.ViewModels.Tabs
{
    public class GeneralViewModel : ReactiveObject
    {
        public GeneralViewModel()
        {
            PauseCommand = ReactiveCommand.Create(PauseTask, this.WhenAnyValue(vm => vm.IsNotPaused));
            ResumeCommand = ReactiveCommand.Create(ResumeTask, this.WhenAnyValue(vm => vm.IsPaused));
            RestartCommand = ReactiveCommand.Create(RestartTask, this.WhenAnyValue(vm => vm.IsPaused));
        }

        public void LoadData(int index)
        {
        }

        private void PauseTask()
        {
        }

        private void ResumeTask()
        {
        }

        private void RestartTask()
        {
        }

        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> ResumeCommand { get; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; }

        private string _status;

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        private string _clickDelay;

        public string ClickDelay
        {
            get => _clickDelay;
            set => this.RaiseAndSetIfChanged(ref _clickDelay, value);
        }

        private string _clickDelayRange;

        public string ClickDelayRange
        {
            get => _clickDelayRange;
            set => this.RaiseAndSetIfChanged(ref _clickDelayRange, value);
        }

        private string _taskDelay;

        public string TaskDelay
        {
            get => _taskDelay;
            set => this.RaiseAndSetIfChanged(ref _taskDelay, value);
        }

        private string _taskDelayRange;

        public string TaskDelayRange
        {
            get => _taskDelayRange;
            set => this.RaiseAndSetIfChanged(ref _taskDelayRange, value);
        }

        private string _workTime;

        public string WorkTime
        {
            get => _workTime;
            set => this.RaiseAndSetIfChanged(ref _workTime, value);
        }

        private string _workTimeRange;

        public string WorkTimeRange
        {
            get => _workTimeRange;
            set => this.RaiseAndSetIfChanged(ref _workTimeRange, value);
        }

        private string _sleepTime;

        public string SleepTime
        {
            get => _sleepTime;
            set => this.RaiseAndSetIfChanged(ref _sleepTime, value);
        }

        private string _sleepTimeRange;

        public string SleepTimeRange
        {
            get => _sleepTimeRange;
            set => this.RaiseAndSetIfChanged(ref _sleepTimeRange, value);
        }

        private bool _isDontLoadImage;

        public bool IsDontLoadImage
        {
            get => _isDontLoadImage;
            set => this.RaiseAndSetIfChanged(ref _isDontLoadImage, value);
        }

        private bool _isMinimized;

        public bool IsMinimized
        {
            get => _isMinimized;
            set => this.RaiseAndSetIfChanged(ref _isMinimized, value);
        }

        private bool _isClosedIfNoTask;

        public bool IsClosedIfNoTask
        {
            get => _isClosedIfNoTask;
            set => this.RaiseAndSetIfChanged(ref _isClosedIfNoTask, value);
        }

        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                this.RaiseAndSetIfChanged(ref _isPaused, value);
                IsNotPaused = !value;
            }
        }

        private bool _isNotPause = true;

        public bool IsNotPaused
        {
            get => _isNotPause;
            set => this.RaiseAndSetIfChanged(ref _isNotPause, value);
        }
    }
}