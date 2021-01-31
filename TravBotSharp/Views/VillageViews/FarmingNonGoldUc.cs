using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;

using TravBotSharp.Interfaces;
using TravBotSharp.Files.Helpers;

using TbsCore.Models.VillageModels;
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

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            if (acc.AccInfo.Tribe != null)
            {
                comboBox_TroopToFarm.Items.Clear();
                int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
                for (var i = troopsEnum + 1; i < troopsEnum + 11; i++)
                {
                    Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                    comboBox_TroopToFarm.Items.Add(VillageHelper.EnumStrToString(troop.ToString()));
                }
                if (comboBox_TroopToFarm.Items.Count > 0) comboBox_TroopToFarm.SelectedIndex = 0;
            }
            else
            {
                comboBox_TroopToFarm.Items.Clear();
            }

            comboBox_NameList.Items.Clear();
            for (var i = 0; i < vill.FarmingNonGold.ListFarm.Count; i++)
            {
                comboBox_NameList.Items.Add(vill.FarmingNonGold.ListFarm[i].Name);
            }

            farmingList.Items.Clear();
        }

        private void loadFarmList(int index)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            var targets = vill.FarmingNonGold.ListFarm[index].Targets;

            farmingList.Items.Clear();
            for (var i = 0; i < targets.Count; i++)
            {
                addFarm2ViewList(acc, targets[i]);
            }
        }

        private void addFarm2ViewList(Account acc, Farm farm)
        {
            // i dont know why Id column cannot show value, it always shifts to right
            ListViewItem item = new ListViewItem();
            item.SubItems.Add(farm.coord.x.ToString());
            item.SubItems.Add(farm.coord.y.ToString());

            if (acc.AccInfo.Tribe != null)
            {
                int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
                Classificator.TroopsEnum troop = (Classificator.TroopsEnum)(troopsEnum + 1 + farm.Troop);

                item.SubItems.Add(VillageHelper.EnumStrToString(troop.ToString()));
            }
            else
            {
                item.SubItems.Add("Don't know");
            }

            item.SubItems.Add(farm.Amount.ToString());
            item.ForeColor = Color.White;

            farmingList.Items.Add(item);
        }

        private void comboBox_NameList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            currentFarmList_index = comboBox_NameList.SelectedIndex;

            loadFarmList(currentFarmList_index);
        }

        private void farmingList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (farmingList.FocusedItem == null) return;
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            var farm = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index];

            X.Value = farm.coord.x;
            Y.Value = farm.coord.y;
            comboBox_TroopToFarm.SelectedIndex = farm.Troop;
            Amount.Value = farm.Amount;
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

            var coord = new Coordinates();
            coord.x = (int)X.Value;
            coord.y = (int)Y.Value;

            var farm = new Farm();
            farm.Troop = comboBox_TroopToFarm.SelectedIndex;
            farm.Amount = (int)Amount.Value;
            farm.coord = coord;

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.Add(farm);

            addFarm2ViewList(acc, farm);
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

            var coord = new Coordinates();
            coord.x = (int)X.Value;
            coord.y = (int)Y.Value;

            var farm = new Farm();
            farm.Troop = comboBox_TroopToFarm.SelectedIndex;
            farm.Amount = (int)Amount.Value;
            farm.coord = coord;

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index] = farm;

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
            vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets.Clear();

            loadFarmList(currentFarmList_index);
        }

        /// <summary>
        /// Attack current show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            Farm f = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets[farmingList.FocusedItem.Index];

            // send troops
        }

        /// <summary>
        /// Attack all targets in current farm list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            var targets = vill.FarmingNonGold.ListFarm[currentFarmList_index].Targets;
            for (int i = 0; i < targets.Count; i++)
            {
                Farm f = targets[i];
                // send troops like above

                // do we need wait task above finish ?
                // and how do we do that ?
            }
        }
    }
}