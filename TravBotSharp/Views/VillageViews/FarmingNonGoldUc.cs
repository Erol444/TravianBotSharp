using System.Windows.Forms;

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

            for (var i = 0; i < vill.FarmingNonGold.ListFarm.Count; i++)
            {
                comboBox_NameList.Items.Add(vill.FarmingNonGold.ListFarm[i].Name);
            }

            loadFarmList(0);
        }

        private void loadFarmList(int index)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            var targets = vill.FarmingNonGold.ListFarm[index].Targets;

            farmingList.Clear();
            for (var i = 0; i < targets.Count; i++)
            {
                addFarm2ViewList(acc, targets[i]);
            }
        }

        private void addFarm2ViewList(Account acc, Farm farm)
        {
            var item = new ListViewItem();

            item.SubItems[0].Text = (farmingList.Items.Count).ToString();
            item.SubItems[1].Text = farm.coord.x.ToString();
            item.SubItems[2].Text = farm.coord.y.ToString();

            if (acc.AccInfo.Tribe != null)
            {
                int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
                Classificator.TroopsEnum troop = (Classificator.TroopsEnum)(troopsEnum + 1 + farm.Troop);

                item.SubItems[3].Text = VillageHelper.EnumStrToString(troop.ToString());
            }
            else
            {
                item.SubItems[3].Text = "Don't know";
            }

            item.SubItems[4].Text = farm.Amount.ToString();

            farmingList.Items.Add(item);
        }

        private void comboBox_NameList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var index = comboBox_NameList.SelectedIndex;
            loadFarmList(index);
        }

        /// <summary>
        ///  New
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, System.EventArgs e)
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

            var list = new FarmList();
            list.Name = comboBox_NameList.Text;
            list.Targets.Add(farm);

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            vill.FarmingNonGold.ListFarm.Add(list);

            UpdateUc();
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

            var list = new FarmList();
            list.Name = comboBox_NameList.Text;
            list.Targets.Add(farm);

            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            vill.FarmingNonGold.ListFarm.Find(f => f.Name == comboBox_NameList.Text).Targets.Add(farm);

            addFarm2ViewList(acc, farm);
        }
    }
}