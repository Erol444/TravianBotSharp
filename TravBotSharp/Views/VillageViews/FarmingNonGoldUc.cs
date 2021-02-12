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
                AddFarmToViewList(targets[i]);
            }
        }

        private void AddFarmToViewList(Farm farm)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text = (farmingList.Items.Count + 1).ToString();
            item.SubItems.Add(farm.Coord.x.ToString());
            item.SubItems.Add(farm.Coord.y.ToString());

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
                form.Farm = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index];
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index] = form.Farm;
                    UpdateFarmList(currentFarmList_index);
                    UpdateFarmTroops();
                }
            }
        }

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

            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.RemoveAt(farmingList.FocusedItem.Index);

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

            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.Clear();

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

            SendTroops taskSendTroops;
            foreach(var f in vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets)
            {
                taskSendTroops = new SendTroops()
                {
                    ExecuteAt = DateTime.Now,
                    Vill = vill,
                    TroopsMovement = new TroopsSendModel()
                    {
                        Coordinates = f.Coord,
                        Troops = f.Troops,
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

            var farm = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index];
            troopsSelectorUc1.Troops = farm.Troops;
        }
    }
}