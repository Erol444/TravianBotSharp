using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Models.TroopsModels;
using TbsCore.Helpers;

using TbsCore.Tasks.LowLevel;
using TbsCore.Tasks.SecondLevel;
using TravBotSharp.Interfaces;
using TravBotSharp.Forms;

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
            acc.Tasks.Add(new SendFLs() { ExecuteAt = DateTime.Now }, true);
        }

        private void trainTroopsAfterFLcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Farming.TrainTroopsAfterFL = trainTroopsAfterFLcheckbox.Checked;
        }

        private void button1_Click(object sender, EventArgs e) //refresh FLs
        {
            GetSelectedAcc().Tasks.Add(new UpdateFarmLists() { ExecuteAt = DateTime.Now }, true);
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

        /// <summary>
        /// more farm open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();

            // This feature is not available for TTWars
            if (acc.AccInfo.ServerVersion != Classificator.ServerVersionEnum.T4_5)
            {
                MessageUser("This feature is only available for normal travian servers.");
                return;
            }

            var fl = GetSelectedFL();
            if (fl == null)
            {
                MessageUser("No FarmList selected!");
                return;
            }

            var label = $"Inactive farm finder for the (Goldclub) Farm List {fl.Name}";
            using (var form = new InactiveFinder(acc, label))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (var item in form.InactiveFarms)
                    {
                        acc.Tasks.Add(new AddFarm()
                        {
                            Farm = item,
                            FarmListId = fl.Id,
                        });
                    };
                }
            }
        }

        private void MessageUser(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButtons.OK);
    }
}