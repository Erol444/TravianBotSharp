using MainCore.Helper;
using ReactiveUI;
using System.Windows;

namespace WPFUI.Models
{
    public class AccountSetting : ReactiveObject
    {
        public void CopyFrom(MainCore.Models.Database.AccountSetting settings)
        {
            ClickDelay = $"{(settings.ClickDelayMin + settings.ClickDelayMax) / 2}";
            ClickDelayRange = $"{(settings.ClickDelayMax - settings.ClickDelayMin) / 2}";
            TaskDelay = $"{(settings.TaskDelayMin + settings.TaskDelayMax) / 2}";
            TaskDelayRange = $"{(settings.TaskDelayMax - settings.TaskDelayMin) / 2}";
            WorkTime = $"{(settings.WorkTimeMax + settings.WorkTimeMin) / 2}";
            WorkTimeRange = $"{(settings.WorkTimeMax - settings.WorkTimeMin) / 2}";
            SleepTime = $"{(settings.SleepTimeMax + settings.SleepTimeMin) / 2}";
            SleepTimeRange = $"{(settings.SleepTimeMax - settings.SleepTimeMin) / 2}";
            IsDontLoadImage = settings.IsDontLoadImage;
            IsClosedIfNoTask = settings.IsClosedIfNoTask;
            IsMinimized = settings.IsMinimized;
            IsAutoStartAdventure = settings.IsAutoAdventure;
        }

        public void CopyTo(MainCore.Models.Database.AccountSetting settings)
        {
            var clickDelay = int.Parse(ClickDelay);
            var clickDelayRange = int.Parse(ClickDelayRange);
            settings.ClickDelayMin = clickDelay - clickDelayRange;
            if (settings.ClickDelayMin < 0) settings.ClickDelayMin = 0;
            settings.ClickDelayMax = clickDelay + clickDelayRange;

            var taskDelay = int.Parse(TaskDelay);
            var taskDelayRange = int.Parse(TaskDelayRange);
            settings.TaskDelayMin = taskDelay - taskDelayRange;
            if (settings.TaskDelayMin < 0) settings.TaskDelayMin = 0;
            settings.TaskDelayMax = taskDelay + taskDelayRange;

            var workTime = int.Parse(WorkTime);
            var workTimeRange = int.Parse(WorkTimeRange);
            settings.WorkTimeMin = workTime - workTimeRange;
            if (settings.WorkTimeMin < 0) settings.WorkTimeMin = 0;
            settings.WorkTimeMax = workTime + workTimeRange;

            var sleepTime = int.Parse(SleepTime);
            var sleepTimeRange = int.Parse(SleepTimeRange);
            settings.SleepTimeMin = sleepTime - sleepTimeRange;
            if (settings.SleepTimeMin < 0) settings.SleepTimeMin = 0;
            settings.SleepTimeMax = sleepTime + sleepTimeRange;

            settings.IsDontLoadImage = IsDontLoadImage;
            settings.IsClosedIfNoTask = IsClosedIfNoTask;
            settings.IsMinimized = IsMinimized;
            settings.IsAutoAdventure = IsAutoStartAdventure;
        }

        public bool IsVaild()
        {
            if (!ClickDelay.IsNumeric())
            {
                MessageBox.Show("Click delay is not a number.", "Warning");
                return false;
            }
            if (!ClickDelayRange.IsNumeric())
            {
                MessageBox.Show("Click delay range is not a number.", "Warning");
                return false;
            }
            if (!TaskDelay.IsNumeric())
            {
                MessageBox.Show("Task delay is not a number.", "Warning");
                return false;
            }
            if (!TaskDelayRange.IsNumeric())
            {
                MessageBox.Show("Task delay range is not a number.", "Warning");
                return false;
            }
            if (!WorkTime.IsNumeric())
            {
                MessageBox.Show("Work time is not a number.", "Warning");
                return false;
            }
            if (!WorkTimeRange.IsNumeric())
            {
                MessageBox.Show("Work time range is not a number.", "Warning");
                return false;
            }
            if (!SleepTime.IsNumeric())
            {
                MessageBox.Show("Sleep time is not a number.", "Warning");
                return false;
            }
            if (!SleepTimeRange.IsNumeric())
            {
                MessageBox.Show("Sleep time range is not a number.", "Warning");
                return false;
            }
            return true;
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

        private bool _isAutoStartAdventure;

        public bool IsAutoStartAdventure
        {
            get => _isAutoStartAdventure;
            set => this.RaiseAndSetIfChanged(ref _isAutoStartAdventure, value);
        }
    }
}