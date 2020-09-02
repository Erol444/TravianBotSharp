using System;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Views
{
    public partial class TroopsUc : UserControl
    {
        ControlPanel main;
        public TroopsUc()
        {
            InitializeComponent();
        }
        public void Init(ControlPanel _main)
        {
            main = _main;
        }
        private Account getSelectedAcc()
        {
            return main != null ? main.GetSelectedAcc() : null;
        }
        public Village getSelectedVillage(Account acc = null)
        {
            return main != null ? main.GetSelectedVillage(acc) : null;
        }
        public void UpdateTroopTab()
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage(acc);
            if (acc.AccInfo.Tribe != null)
            {
                if (vill.Troops.TroopToTrain != null)
                    labelTroopsToTrain.Text = $"Selected: {VillageHelper.EnumStrToString(vill.Troops.TroopToTrain.ToString() ?? "")}";
                else labelTroopsToTrain.Text = "Selected:";

                comboBoxTroopsToTrain.Items.Clear();
                int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
                for (var i = troopsEnum + 1; i < troopsEnum + 11; i++)
                {
                    Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                    comboBoxTroopsToTrain.Items.Add(VillageHelper.EnumStrToString(troop.ToString()));
                }
                if (comboBoxTroopsToTrain.Items.Count > 0) comboBoxTroopsToTrain.SelectedIndex = 0;
            }
            else
            {
                labelTroopsToTrain.Text = "Selected:";
                comboBoxTroopsToTrain.Items.Clear();
            }

            autoImprove.Checked = acc.Settings.AutoImprove;
        }

        private void button10_Click(object sender, EventArgs e) //select troop to train
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage();
            int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
            var troopSelected = troopsEnum + comboBoxTroopsToTrain.SelectedIndex + 1;
            vill.Troops.TroopToTrain = (Classificator.TroopsEnum)troopSelected;
            labelTroopsToTrain.Text = $"Selected: {VillageHelper.EnumStrToString(vill.Troops.TroopToTrain.ToString() ?? "")}";
        }

        private void autoImprove_CheckedChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.AutoImprove = autoImprove.Checked;
        }
    }
}
