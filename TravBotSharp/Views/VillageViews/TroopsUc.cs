using System;
using System.Collections.Generic;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class TroopsUc : BaseVillageUc, ITbsUc
    {
        public TroopsUc()
        {
            InitializeComponent();
        }
        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
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

            // Village troops info
            string infoText = "-- Troops already researched:\n";
            infoText += string.Join(", ", vill.Troops.Researched) + "\n";
            infoText += "-- Troops to be researched:\n";
            infoText += string.Join(", ", vill.Troops.ToResearch) + "\n";
            infoText += "-- Troop smithy levels:\n";

            List<string> levels = new List<string>();
            foreach (var level in vill.Troops.Levels)
            {
                levels.Add(level.Troop + ": " + level.Level);
            }
            infoText += string.Join(", ", levels) + "\n";

            infoText += "-- Troop to be improved:\n";
            infoText += string.Join(", ", vill.Troops.ToImprove) + "\n";
            infoText += $"-- Settlers already trained: {vill.Troops.Settlers}";

            //List<string> ctStr = new List<string>();
            //foreach(var ct in vill.Troops.CurrentlyTraining.)
            //{
            //    ctStr.Add(ct.)
            //}

            troopsInfo.Text = infoText;
        }

        private void button10_Click(object sender, EventArgs e) //select troop to train
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();
            int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
            var troopSelected = troopsEnum + comboBoxTroopsToTrain.SelectedIndex + 1;
            vill.Troops.TroopToTrain = (Classificator.TroopsEnum)troopSelected;
            labelTroopsToTrain.Text = $"Selected: {VillageHelper.EnumStrToString(vill.Troops.TroopToTrain.ToString() ?? "")}";
        }
    }
}
