using System;
using System.Collections.Generic;
using TbsCore.Helpers;
using TbsCore.Tasks.SecondLevel;
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

            troopSelectorTrain.Init(acc.AccInfo.Tribe);
            troopSelectorTrain.SelectedTroop = vill.Troops.TroopToTrain;

            troopSelectorImprove.Init(acc.AccInfo.Tribe);

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

            troopsInfo.Text = infoText;
        }

        private void button10_Click(object sender, EventArgs e) //select troop to train
        {
            GetSelectedVillage().Troops.TroopToTrain = troopSelectorTrain.SelectedTroop;
        }

        private void button1_Click(object sender, EventArgs e) // Add to improve
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            vill.Troops.ToImprove.Add(troopSelectorImprove.SelectedTroop ?? Classificator.TroopsEnum.None);
            TroopsHelper.ReStartResearchAndImprovement(acc, vill);
            this.UpdateUc();
        }

        private void button2_Click(object sender, EventArgs e) // scouts
        {
            GetSelectedAcc().Tasks.Add(new SendReinforcementScouts
            {
                Scouts = (int)scouts.Value,
                Vill = GetSelectedVillage()
            }, true);
        }
    }
}