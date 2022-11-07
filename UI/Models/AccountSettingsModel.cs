using FluentResults;
using MainCore.Models.Database;
using ReactiveUI;

namespace UI.Models
{
    public class AccountSettingsModel : ReactiveObject
    {
        public void CopyFrom(AccountSetting settings)
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
        }

        public void CopyTo(AccountSetting settings)
        {
            settings.ClickDelayMin = ClickDelay - ClickDelayRange;
            settings.ClickDelayMax = ClickDelay + ClickDelayRange;
            settings.TaskDelayMin = TaskDelay - TaskDelayRange;
            settings.TaskDelayMax = TaskDelay + TaskDelayRange;
            settings.WorkTimeMin = WorkTime - WorkTimeRange;
            settings.WorkTimeMax = WorkTime + WorkTimeRange;
            settings.SleepTimeMin = SleepTime - SleepTimeRange;
            settings.SleepTimeMax = SleepTime + SleepTimeRange;
            settings.IsDontLoadImage = IsDontLoadImage;
            settings.IsClosedIfNoTask = IsClosedIfNoTask;
            settings.IsMinimized = IsMinimized;
            settings.IsAutoAdventure = IsAutoStartAdventure;
        }

        public Result IsValid()
        {
            if (ClickDelay < ClickDelayRange) return Result.Fail("Click delay tolerance number should be smaller than click delay value");
            if (SleepTime < SleepTimeRange) return Result.Fail("Sleep time tolerance number should be smaller than sleep time value");
            if (WorkTime < WorkTimeRange) return Result.Fail("Work time tolerance number should be smaller than work time value");
            return Result.Ok();
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
    }
}