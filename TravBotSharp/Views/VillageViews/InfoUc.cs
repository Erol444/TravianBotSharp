using System;
using System.Collections.Generic;
using TbsCore.Extensions;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Interfaces;

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

            string infoText = $"-- Vill stored res\n{vill.Res.Stored.Resources}\n";
            infoText += $"-- Vill resource production\n{vill.Res.Production} (per hour)\n";
            infoText += $"-- Vill capacity\nWarehouse:{vill.Res.Capacity.WarehouseCapacity}, Granary: {vill.Res.Capacity.GranaryCapacity}\n";

            infoText += "-- Village's unfinished tasks (due to low res)\n";
            if(vill.UnfinishedTasks != null)
            {
                List<string> unfinishedTasks = new List<string>();
                foreach (var tasks in vill.UnfinishedTasks)
                {
                    unfinishedTasks.Add(tasks.Task.GetName() + " - Needed " + tasks.ResNeeded.ToString());
                }
                infoText += string.Join("\n ", unfinishedTasks);
            }

            infoText += "-- Village next update\n";
            infoText += vill.Timings.NextVillRefresh + "\n";

            villageInfo.Text = infoText;
        }
    }
}
