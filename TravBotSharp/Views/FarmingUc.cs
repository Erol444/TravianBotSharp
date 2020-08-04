using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Files.Tasks.SecondLevel;

namespace TravBotSharp.Views
{
    public partial class FarmingUc : UserControl
    {
        ControlPanel main;
        public FarmingUc()
        {
            InitializeComponent();
            RaidStyle.Items.AddRange(new string[] { "No losses only", "Some losses", "All losses" });
        }
        public void Init(ControlPanel _main)
        {
            main = _main;
        }
        public void UpdateTab()
        {
            var acc = main.GetSelectedAcc();
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
            var acc = main.GetSelectedAcc();
            acc.Farming.MinInterval = (int)minFarmInterval.Value;
            acc.Farming.MaxInterval = (int)maxFarmInterval.Value;
            acc.Farming.Enabled = true;
            TaskExecutor.AddTaskIfNotExists(acc, new SendFLs() { ExecuteAt = DateTime.Now });
        }

        private void trainTroopsAfterFLcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            main.GetSelectedAcc().Farming.TrainTroopsAfterFL = trainTroopsAfterFLcheckbox.Checked;
        }

        private void button1_Click(object sender, EventArgs e) //refresh FLs
        {
            TaskExecutor.AddTaskIfNotExists(main.GetSelectedAcc(), new UpdateFarmLists() { ExecuteAt = DateTime.Now });
        }

        private int GetSelectedFL()
        {
            return FlCombo.SelectedIndex;
        }
        private void UpdateFlInfo()
        {
            var acc = main.GetSelectedAcc();

            var fl = acc.Farming.FL[GetSelectedFL()];
            FlName.Text = fl.Name;
            RaidStyle.SelectedIndex = (int)fl.RaidStyle;
            FarmNum.Text = fl.NumOfFarms.ToString();
            FlEnabled.Checked = fl.Enabled;
        }
        private void RaidStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            //save raid style
            var acc = main.GetSelectedAcc();
            acc.Farming.FL[GetSelectedFL()].RaidStyle = (Files.Models.TroopsModels.RaidStyle)RaidStyle.SelectedIndex;
        }

        private void FlCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFlInfo();
        }

        private void FlEnabled_CheckedChanged(object sender, EventArgs e)
        {
            var acc = main.GetSelectedAcc();
            acc.Farming.FL[GetSelectedFL()].Enabled = FlEnabled.Checked;
        }

        private void button2_Click(object sender, EventArgs e) //add natar vilalges to this FL
        {
            var acc = main.GetSelectedAcc();
            var task = new TTWarsAddNatarsToFL
            {
                FL = acc.Farming.FL[GetSelectedFL()],
                ExecuteAt = DateTime.Now,
                MaxPop = (int)maxPopNatar.Value,
                MinPop = (int)minPopNatar.Value
            };
            TaskExecutor.AddTask(acc, task);
        }

        private void StopFarm_Click(object sender, EventArgs e) // Stop farming
        {
            var acc = main.GetSelectedAcc();
            acc.Farming.Enabled = false;
            //remove all SendFarmlist tasks
            var flTasks = acc.Tasks.Where(x => x.GetType() == typeof(SendFLs) || x.GetType() == typeof(SendFarmlist)).ToList();
            while (flTasks.Count > 0)
            {
                acc.Tasks.Remove(flTasks[0]);
                flTasks.RemoveAt(0);
            }
        }
    }
}
