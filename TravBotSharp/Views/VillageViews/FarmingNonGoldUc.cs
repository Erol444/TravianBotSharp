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

namespace TravBotSharp.Views
{
    public partial class FarmingNonGoldUc : BaseVillageUc, ITbsUc
    {
        public FarmingNonGoldUc()
        {
            InitializeComponent();
        }

        private int currentFarmList_index;
        private Farm currentFarm;

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            if (vill == null) return;

            if (acc.AccInfo.Tribe != null)
            {
                currentFarm = new Farm();
                comboBox_TroopToFarm.Items.Clear();
                troopList.Items.Clear();

                int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;

                for (var i = troopsEnum + 1; i < troopsEnum + 7; i++)
                {
                    Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                    var type = VillageHelper.EnumStrToString(troop.ToString());
                    comboBox_TroopToFarm.Items.Add(type);

                    var item = new ListViewItem();
                    item.SubItems[0].Text = (i - troopsEnum).ToString();
                    item.SubItems.Add(type);
                    item.SubItems.Add("0");
                    item.ForeColor = Color.White;
                    troopList.Items.Add(item);
                }
                if (comboBox_TroopToFarm.Items.Count > 0) comboBox_TroopToFarm.SelectedIndex = 0;
            }
            else
            {
                comboBox_TroopToFarm.Items.Clear();
                troopList.Items.Clear();
            }

            comboBox_NameList.Items.Clear();
            for (var i = 0; i < vill.FarmingNonGold.ListFarm.Count; i++)
            {
                comboBox_NameList.Items.Add(vill.FarmingNonGold.ListFarm[i].Name);
            }
            if (vill.FarmingNonGold.ListFarm.Count > 0)
            {
                comboBox_NameList.SelectedIndex = 0;
                loadFarmList(0);
            }
            else
            {
                farmingList.Items.Clear();
            }
            currentFarm = new Farm();
        }

        private void loadFarmList(int index)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;

            var targets = vill.FarmingNonGold.ListFarm[index].Targets;

            farmingList.Items.Clear();
            for (var i = 0; i < targets.Count; i++)
            {
                addFarm2ViewList(acc, targets[i]);
            }
        }

        private void addFarm2ViewList(Account acc, Farm farm)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text = (farmingList.Items.Count + 1).ToString();
            item.SubItems.Add(farm.Coord.x.ToString());
            item.SubItems.Add(farm.Coord.y.ToString());

            item.ForeColor = Color.White;

            farmingList.Items.Add(item);
        }

        /// <summary>
        ///  New
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;

            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.Add(new Farm(currentFarm));
            addFarm2ViewList(acc, currentFarm);
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, System.EventArgs e)
        {
            if (comboBox_NameList.Text == "")
            {
                return;
            }

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;
            if (troopList.FocusedItem == null) return;

            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index] = new Farm(currentFarm);

            loadFarmList(currentFarmList_index);
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

            loadFarmList(currentFarmList_index);
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

            loadFarmList(currentFarmList_index);
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
            Farm f;
            var targets = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets;
            for (int i = 0; i < targets.Count; i++)
            {
                f = targets[i];
                taskSendTroops = new SendTroops()
                {
                    ExecuteAt = DateTime.Now,
                    Vill = vill,
                    TroopsMovement = new TroopsSendModel()
                    {
                        Coordinates = new Coordinates()
                        {
                            x = f.Coord.x,
                            y = f.Coord.y
                        },
                        Troops = new int[12]
                    }
                };

                for (int index = 0; index < f.Troops.Length; index++)
                {
                    taskSendTroops.TroopsMovement.Troops[index] = f.Troops[index];
                }

                taskSendTroops.TroopsMovement.MovementType = Classificator.MovementType.Raid;

                TaskExecutor.AddTask(acc, taskSendTroops);
            }
        }

        private void comboBox_NameList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            currentFarmList_index = comboBox_NameList.SelectedIndex;

            loadFarmList(currentFarmList_index);
        }

        private void farmingList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (vill == null) return;

            if (farmingList.FocusedItem == null) return;
            var farm = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index];

            currentFarm = new Farm(farm);

            X.Value = farm.Coord.x;
            Y.Value = farm.Coord.y;

            for (int i = 0; i < farm.Troops.Length; i++)
            {
                troopList.Items[i].SubItems[2].Text = farm.Troops[i].ToString();
            }

            comboBox_TroopToFarm.SelectedIndex = 0;
        }

        private void comboBox_TroopToFarm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Amount.Value = currentFarm.Troops[comboBox_TroopToFarm.SelectedIndex];
        }

        private void X_ValueChanged(object sender, System.EventArgs e)
        {
            currentFarm.Coord.x = (int)X.Value;
        }

        private void Y_ValueChanged(object sender, System.EventArgs e)
        {
            currentFarm.Coord.y = (int)Y.Value;
        }

        private void Amount_ValueChanged(object sender, System.EventArgs e)
        {
            currentFarm.Troops[comboBox_TroopToFarm.SelectedIndex] = (int)Amount.Value;
            troopList.Items[comboBox_TroopToFarm.SelectedIndex].SubItems[2].Text = Amount.Value.ToString();
        }
    }
}