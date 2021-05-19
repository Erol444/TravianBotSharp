﻿using System;
using System.Collections.Generic;
using TbsCore.Extensions;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Interfaces;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Views
{
    public partial class InfoUc : BaseVillageUc, ITbsUc
    {
        public InfoUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var vill = GetSelectedVillage();

            minInterval.Value = vill.Settings.RefreshMin;
            maxInterval.Value = vill.Settings.RefreshMax;

            string infoText = $"-- Vill stored res\n{vill.Res.Stored.Resources}\n";
            infoText += $"-- Vill resource production\n{vill.Res.Production} (per hour)\n";
            infoText += $"-- Vill capacity\nWarehouse:{vill.Res.Capacity.WarehouseCapacity}, Granary: {vill.Res.Capacity.GranaryCapacity}\n";

            infoText += "-- Village's unfinished tasks (due to low res)\n";
            if (vill.UnfinishedTasks != null)
            {
                foreach (var tasks in vill.UnfinishedTasks)
                {
                    infoText += $"{tasks.Task.GetName()} - Needed {tasks.ResNeeded}\n";
                }
            }

            villageInfo.Text = infoText;
        }

        private void minInterval_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Settings.RefreshMin = (int)minInterval.Value;

        private void maxInterval_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Settings.RefreshMax = (int)maxInterval.Value;

        private void TrainSettlers_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new TrainSettlers()
            {
                ExecuteAt = DateTime.Now.AddHours(-2),
                Vill = vill
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new SendSettlers()
            {
                ExecuteAt = DateTime.Now.AddHours(-2),
                Vill = vill
            });
        }
    }
}