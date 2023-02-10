using ReactiveUI;

namespace WPFUI.Models
{
    public class AccountSetting : ReactiveObject
    {
        public void CopyFrom(MainCore.Models.Database.AccountSetting settings)
        {
            ClickDelay = (settings.ClickDelayMin + settings.ClickDelayMax) / 2;
            ClickDelayRange = (settings.ClickDelayMax - settings.ClickDelayMin) / 2;
            TaskDelay = (settings.TaskDelayMin + settings.TaskDelayMax) / 2;
            TaskDelayRange = (settings.TaskDelayMax - settings.TaskDelayMin) / 2;
            WorkTime = (settings.WorkTimeMax + settings.WorkTimeMin) / 2;
            WorkTimeRange = (settings.WorkTimeMax - settings.WorkTimeMin) / 2;
            SleepTime = (settings.SleepTimeMax + settings.SleepTimeMin) / 2;
            SleepTimeRange = (settings.SleepTimeMax - settings.SleepTimeMin) / 2;
            IsDontLoadImage = settings.IsDontLoadImage;
            IsClosedIfNoTask = settings.IsClosedIfNoTask;
            IsMinimized = settings.IsMinimized;
            IsAutoStartAdventure = settings.IsAutoAdventure;
            IsSleepBetweenProxyChanging = settings.IsSleepBetweenProxyChanging;
        }

        public void CopyTo(MainCore.Models.Database.AccountSetting settings)
        {
            settings.ClickDelayMin = ClickDelay - ClickDelayRange;
            if (settings.ClickDelayMin < 0) settings.ClickDelayMin = 0;
            settings.ClickDelayMax = ClickDelay + ClickDelayRange;

            settings.TaskDelayMin = TaskDelay - TaskDelayRange;
            if (settings.TaskDelayMin < 0) settings.TaskDelayMin = 0;
            settings.TaskDelayMax = TaskDelay + TaskDelayRange;

            settings.WorkTimeMin = WorkTime - WorkTimeRange;
            if (settings.WorkTimeMin < 0) settings.WorkTimeMin = 0;
            settings.WorkTimeMax = WorkTime + WorkTimeRange;

            settings.SleepTimeMin = SleepTime - SleepTimeRange;
            if (settings.SleepTimeMin < 0) settings.SleepTimeMin = 0;
            settings.SleepTimeMax = SleepTime + SleepTimeRange;

            settings.IsDontLoadImage = IsDontLoadImage;
            settings.IsClosedIfNoTask = IsClosedIfNoTask;
            settings.IsMinimized = IsMinimized;
            settings.IsAutoAdventure = IsAutoStartAdventure;
            settings.IsSleepBetweenProxyChanging = IsSleepBetweenProxyChanging;
        }

        private int _clickDelay;

        public int ClickDelay
        {
            get => _clickDelay;
            set => this.RaiseAndSetIfChanged(ref _clickDelay, value);
        }

        private int _clickDelayRange;

        public int ClickDelayRange
        {
            get => _clickDelayRange;
            set => this.RaiseAndSetIfChanged(ref _clickDelayRange, value);
        }

        private int _taskDelay;

        public int TaskDelay
        {
            get => _taskDelay;
            set => this.RaiseAndSetIfChanged(ref _taskDelay, value);
        }

        private int _taskDelayRange;

        public int TaskDelayRange
        {
            get => _taskDelayRange;
            set => this.RaiseAndSetIfChanged(ref _taskDelayRange, value);
        }

        private int _workTime;

        public int WorkTime
        {
            get => _workTime;
            set => this.RaiseAndSetIfChanged(ref _workTime, value);
        }

        private int _workTimeRange;

        public int WorkTimeRange
        {
            get => _workTimeRange;
            set => this.RaiseAndSetIfChanged(ref _workTimeRange, value);
        }

        private int _sleepTime;

        public int SleepTime
        {
            get => _sleepTime;
            set => this.RaiseAndSetIfChanged(ref _sleepTime, value);
        }

        private int _sleepTimeRange;

        public int SleepTimeRange
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

        private bool _isAutoStartAdventure;

        public bool IsAutoStartAdventure
        {
            get => _isAutoStartAdventure;
            set => this.RaiseAndSetIfChanged(ref _isAutoStartAdventure, value);
        }

        private bool _isSleepBetweenProxyChanging;

        public bool IsSleepBetweenProxyChanging
        {
            get => _isSleepBetweenProxyChanging;
            set => this.RaiseAndSetIfChanged(ref _isSleepBetweenProxyChanging, value);
        }
    }
}