using System;
using System.Data;
using System.Linq;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.TroopsModels;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Files.Tasks.SecondLevel;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class FarmingUc : TbsBaseUc, ITbsUc
    {
        public FarmingUc()
        {
            InitializeComponent();
            RaidStyle.Items.AddRange(new string[] { "No losses only", "Some losses", "All losses" });
        }
        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            minFarmInterval.Value = acc.Farming.MinInterval;
            maxFarmInterval.Value = acc.Farming.MaxInterval;

            trainTroopsAfterFLcheckbox.Checked = acc.Farming.TrainTroopsAfterFL;

            if (acc.Farming.FL.Count != 0)
            {
                FlCombo.Items.Clear();
                foreach (var fl in acc.Farming.FL) FlCombo.Items.Add(fl.Name);
                FlCombo.SelectedIndex = 0;
                UpdateFlInfo();
            }
        }
        private void StartFarm_Click(object sender, EventArgs e)//start farming
        {
            var acc = GetSelectedAcc();
            acc.Farming.MinInterval = (int)minFarmInterval.Value;
            acc.Farming.MaxInterval = (int)maxFarmInterval.Value;
            acc.Farming.Enabled = true;
            TaskExecutor.AddTaskIfNotExists(acc, new SendFLs() { ExecuteAt = DateTime.Now });
        }

        private void trainTroopsAfterFLcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Farming.TrainTroopsAfterFL = trainTroopsAfterFLcheckbox.Checked;
        }

        private void button1_Click(object sender, EventArgs e) //refresh FLs
        {
            TaskExecutor.AddTaskIfNotExists(GetSelectedAcc(), new UpdateFarmLists() { ExecuteAt = DateTime.Now });
        }

        /// <summary>
        /// Get's selected FarmList
        /// </summary>
        /// <returns>FarmList</returns>
        private FarmList GetSelectedFL()
        {
            var acc = GetSelectedAcc();
            return acc.Farming.FL.ElementAtOrDefault(FlCombo.SelectedIndex) ?? acc.Farming.FL.FirstOrDefault();
        }


        private void UpdateFlInfo()
        {
            var fl = GetSelectedFL();
            FlName.Text = fl.Name;
            RaidStyle.SelectedIndex = (int)fl.RaidStyle;
            FarmNum.Text = fl.NumOfFarms.ToString();
            FlEnabled.Checked = fl.Enabled;
            flInterval.Value = fl.Interval;
        }
        private void RaidStyle_SelectedIndexChanged(object sender, EventArgs e) //save raid style
        {
            GetSelectedFL().RaidStyle = (RaidStyle)RaidStyle.SelectedIndex;
        }

        private void FlCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFlInfo();
        }

        private void FlEnabled_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedFL().Enabled = FlEnabled.Checked;
        }

        private void button2_Click(object sender, EventArgs e) //add natar vilalges to this FL
        {
            var acc = GetSelectedAcc();
            var task = new TTWarsAddNatarsToFL
            {
                FL = GetSelectedFL(),
                ExecuteAt = DateTime.Now,
                MaxPop = (int)maxPopNatar.Value,
                MinPop = (int)minPopNatar.Value
            };
            TaskExecutor.AddTask(acc, task);
        }

        private void StopFarm_Click(object sender, EventArgs e) // Stop farming
        {
            var acc = GetSelectedAcc();
            acc.Farming.Enabled = false;
            //remove all SendFarmlist tasks
            var flTasks = acc.Tasks.Where(x => x.GetType() == typeof(SendFLs) || x.GetType() == typeof(SendFarmlist)).ToList();
            while (flTasks.Count > 0)
            {
                acc.Tasks.Remove(flTasks[0]);
                flTasks.RemoveAt(0);
            }
        }

        private void flInterval_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedFL().Interval = (int)flInterval.Value;
        }
    }
}
