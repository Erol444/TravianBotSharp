using System;
using System.Windows.Forms;
using System.Drawing;

using TravBotSharp.Interfaces;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;

using TbsCore.Models.VillageModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.AccModels;
using TravBotSharp.Forms;

namespace TravBotSharp.Views
{
    public partial class FarmingNonGoldUc : BaseVillageUc, ITbsUc
    {
        public FarmingNonGoldUc()
        {
            InitializeComponent();

            troopsSelectorUc1.TroopsEditable = false;
            troopsSelectorUc1.HeroEditable = false;
        }

        private int currentFarmList_index;

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            if (vill == null) return;

            comboBox_NameList.Items.Clear();
            for (var i = 0; i < vill.FarmingNonGold.ListFarm.Count; i++)
            {
                comboBox_NameList.Items.Add(vill.FarmingNonGold.ListFarm[i].Name);
            }
            if (vill.FarmingNonGold.ListFarm.Count > 0)
            {
                comboBox_NameList.SelectedIndex = 0;
                UpdateFarmList(0);
            }
            else
            {
                farmingList.Items.Clear();
            }

            troopsSelectorUc1.Init(acc.AccInfo.Tribe);
        }

        /// <summary>
        /// Updates FarmList ListView
        /// </summary>
        /// <param name="index">FarmList index</param>
        private void UpdateFarmList(int index)
        {
            var vill = GetSelectedVillage();
            if (vill == null) return;

            var targets = vill.FarmingNonGold.ListFarm[index].Targets;

            farmingList.Items.Clear();
            for (var i = 0; i < targets.Count; i++)
            {
                try { AddFarmToViewList(targets[i]); }
                catch { }
            }
        }

        private void AddFarmToViewList(Farm farm)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text = (farmingList.Items.Count + 1).ToString();
            item.SubItems.Add(farm.Coords.x.ToString());
            item.SubItems.Add(farm.Coords.y.ToString());

            item.ForeColor = Color.White;

            farmingList.Items.Add(item);
        }

        /// <summary>
        /// Create new FarmList
        /// </summary>
        private void button1_Click(object sender, System.EventArgs e)
        {
            var addName = new AddNewFarmListNameForm();
            DialogResult dr = addName.ShowDialog(this);
            if (dr == DialogResult.Cancel)
            {
                addName.Close();
            }
            else if (dr == DialogResult.OK)
            {
                var acc = GetSelectedAcc();
                var vill = GetSelectedVillage(acc);
                if (vill == null) return;

                var farmlist = new FarmList();

                farmlist.Name = addName.getName();
                addName.Close();

                vill.FarmingNonGold.ListFarm.Add(farmlist);
                comboBox_NameList.Items.Add(farmlist.Name);
                comboBox_NameList.SelectedIndex = comboBox_NameList.Items.Count - 1;
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, System.EventArgs e)
        {
            if (comboBox_NameList.Text == "")
            {
                return;
            }

            var vill = GetSelectedVillage();
            if (vill == null) return;

            using (var form = new AddFarmNonGold(GetSelectedAcc().AccInfo.Tribe))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.Add(form.Farm);
                    UpdateFarmList(currentFarmList_index);
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, System.EventArgs e)
        {
            if (comboBox_NameList.Text == "")
            {
                return;
            }

            var vill = GetSelectedVillage();
            if (vill == null) return;

            using (var form = new AddFarmNonGold(GetSelectedAcc().AccInfo.Tribe))
            {
                form.Farm = GetSelectedFarm();
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    GetSelectedFl().Targets[farmingList.FocusedItem.Index] = form.Farm;
                    UpdateFarmList(currentFarmList_index);
                    UpdateFarmTroops();
                }
            }
        }

        private FarmList GetSelectedFl() =>
            GetSelectedVillage().FarmingNonGold.ListFarm[currentFarmList_index];

        private Farm GetSelectedFarm() =>
            GetSelectedFl().Targets[farmingList.FocusedItem.Index];

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, System.EventArgs e)
        {
            if (comboBox_NameList.Text == "")
            {
                return;
            }

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;
            if (farmingList.FocusedItem == null) return;

            GetSelectedFl().Targets.RemoveAt(farmingList.FocusedItem.Index);

            UpdateFarmList(currentFarmList_index);
        }

        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;

            GetSelectedFl().Targets.Clear();

            UpdateFarmList(currentFarmList_index);
        }

        /// <summary>
        /// Attack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;

            var fl = GetSelectedFl();
            for (int i = 0; i < fl.Targets.Count; i++)
            {
                var taskSendTroops = new SendTroops()
                {
                    ExecuteAt = DateTime.Now.AddMilliseconds(i * 15 * AccountHelper.Delay()),
                    Vill = vill,
                    TroopsMovement = new TroopsSendModel()
                    {
                        TargetCoordinates = fl.Targets[i].Coords,
                        Troops = fl.Targets[i].Troops,
                        MovementType = Classificator.MovementType.Raid
                    }
                };

                TaskExecutor.AddTask(acc, taskSendTroops);
            }
        }

        private void comboBox_NameList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            currentFarmList_index = comboBox_NameList.SelectedIndex;

            UpdateFarmList(currentFarmList_index);
        }

        private void farmingList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateFarmTroops();
        }

        /// <summary>
        /// Updates TroopsSelectorUc that displays the troop count of the selected farm
        /// </summary>
        private void UpdateFarmTroops()
        {
            var vill = GetSelectedVillage();
            if (vill == null) return;

            if (farmingList.FocusedItem == null) return;

            troopsSelectorUc1.Troops = GetSelectedFarm().Troops;
        }

        /// <summary>
        /// more farm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox_NameList.Text == "")
            {
                return;
            }
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;

            // This feature is not available for TTWars
            if (acc.AccInfo.ServerVersion != Classificator.ServerVersionEnum.T4_5)
            {
                MessageUser("This feature is only available for normal travian servers.");
                return;
            }

            var fl = GetSelectedFl();
            if (fl == null)
            {
                MessageUser("No FL selected!");
                return;
            }
            var label = $"Inactive farm finder for the (Non-Goldclub) Farm List {fl.Name}";
            using (var form = new InactiveFinder(acc, label))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.AddRange(form.InactiveFarms);

                    UpdateFarmList(currentFarmList_index);
                    UpdateFarmTroops();
                }
            }
        }

        private void MessageUser(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButtons.OK);
    }
}