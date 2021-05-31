﻿using System;
using System.Linq;
using System.Timers;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using TbsCore.Tasks;
using TbsCore.Tasks.LowLevel;
using static TbsCore.Tasks.BotTask;

namespace TbsCore.Models.AccModels
{
    public class TaskTimer : IDisposable
    {
        private readonly Account acc;
        private Timer Timer { get; set; }

        public bool? IsBotRunning() => Timer.Enabled;

        public TaskTimer(Account account)
        {
            acc = account;
            Timer = new Timer(500);
            Timer.Elapsed += TimerElapsed;
            Start();
        }

        public void Start()
        {
            Timer.Start();
            Timer.Enabled = true;
            Timer.AutoReset = true;
        }

        public void Stop()
        {
            Timer.Stop();
            Timer.Enabled = false;
        }

        private void TimerElapsed(Object source, ElapsedEventArgs e) => NewTick();

        private async void NewTick()
        {
            try
            {
                if (acc.Tasks.Count == 0) return; //No tasks

                // Another task is already in progress. wait
                if (acc.Tasks.IsTaskExcuting()) return;

                var tasks = acc.Tasks.GetTasksReady();
                if (tasks.Count == 0)
                {
                    NoTasks(acc);
                    return;
                }

                BotTask firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.High);
                if (firstTask == null) firstTask = tasks.FirstOrDefault(x => x.Priority == TaskPriority.Medium);
                if (firstTask == null) firstTask = tasks.FirstOrDefault();

                firstTask.Stage = TaskStage.Executing;

                //If correct village is selected, otherwise change village
                if (firstTask.Vill != null)
                {
                    var active = acc.Villages.FirstOrDefault(x => x.Active);
                    if (active != null && active != firstTask.Vill)
                    {
                        await VillageHelper.SwitchVillage(acc, firstTask.Vill.Id);
                    }
                }
                await TaskExecutor.Execute(acc, firstTask);
            }
            catch (Exception e)
            {
                acc?.Logger.Error(e, $"Error in TaskTimer!");
            }
        }

        private void NoTasks(Account acc)
        {
            BotTask task = null;

            if (acc.Settings.AutoCloseDriver &&
                TimeSpan.FromMinutes(5) < TimeHelper.NextPrioTask(acc, TaskPriority.Medium))
            {
                // Auto close chrome and reopen when there is a high/normal prio BotTask
                task = new ReopenDriver();
                ((ReopenDriver)task).LowestPrio = TaskPriority.Medium;
            }
            else if (acc.Settings.AutoRandomTasks)
            {
                task = new RandomTask();
            }

            if (task != null)
            {
                task.ExecuteAt = DateTime.Now;
                task.Priority = TaskPriority.Low;
                acc.Tasks.Add(task);
            }
        }

        public void Dispose()
        {
            this.Timer.Dispose();
        }
    }
}