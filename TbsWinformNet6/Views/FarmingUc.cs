using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Helpers;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Tasks.Farming;
using TbsWinformNet6.Interfaces;
using TbsWinformNet6.Views.BaseViews;

namespace TbsWinformNet6.Views
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

            var filtered = acc.Server.FarmScoutReport
                .Where(x => x.GetRaidableRes().Sum() > 40000)
                .OrderBy(x => AccountHelper.GetMainVillage(acc).Coordinates.CalculateDistance(acc, new Coordinates(acc, x.Deffender.VillageId)));
            var str = "";
            foreach (var rep in filtered)
            {
                str += $"{new Coordinates(acc, rep.Deffender.VillageId)} - {rep.Deffender.VillageName} > SUM {rep.GetRaidableRes().Sum()}\n";
            }
            this.scouted.Text = str;
        }

        private void StartFarm_Click(object sender, EventArgs e)//start farming
        {
            var acc = GetSelectedAcc();
            acc.Farming.MinInterval = (int)minFarmInterval.Value;
            acc.Farming.MaxInterval = (int)maxFarmInterval.Value;
            acc.Farming.Enabled = true;
            acc.Tasks.Add(new SendFLs() { ExecuteAt = DateTime.Now.AddHours(-2) }, true);
        }

        private void trainTroopsAfterFLcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Farming.TrainTroopsAfterFL = trainTroopsAfterFLcheckbox.Checked;
        }

        private void button1_Click(object sender, EventArgs e) //refresh FLs
        {
            GetSelectedAcc().Tasks.Add(new UpdateFarmLists() { ExecuteAt = DateTime.Now.AddHours(-1) }, true);
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
                ExecuteAt = DateTime.Now.AddHours(-1),
                MaxPop = (int)maxPopNatar.Value,
                MinPop = (int)minPopNatar.Value
            };
            acc.Tasks.Add(task);
        }

        private void StopFarm_Click(object sender, EventArgs e) // Stop farming
        {
            var acc = GetSelectedAcc();
            acc.Farming.Enabled = false;
            //remove all SendFarmlist / SendFLs tasks
            acc.Tasks.Remove(typeof(SendFLs));
            acc.Tasks.Remove(typeof(SendFarmlist));
        }

        private void flInterval_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedFL().Interval = (int)flInterval.Value;
        }

        private void MessageUser(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButtons.OK);
    }
}