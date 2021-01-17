using System;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class MarketUc : BaseVillageUc, ITbsUc
    {
        public MarketUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var vill = GetSelectedVillage();
            TargetLimitWood.Value = vill.Market.Settings.Configuration.TargetLimit.Wood;
            TargetLimitClay.Value = vill.Market.Settings.Configuration.TargetLimit.Clay;
            TargetLimitIron.Value = vill.Market.Settings.Configuration.TargetLimit.Iron;
            TargetLimitCrop.Value = vill.Market.Settings.Configuration.TargetLimit.Crop;
            FillLimitWood.Value = vill.Market.Settings.Configuration.FillLimit.Wood;
            FillLimitClay.Value = vill.Market.Settings.Configuration.FillLimit.Clay;
            FillLimitIron.Value = vill.Market.Settings.Configuration.FillLimit.Iron;
            FillLimitCrop.Value = vill.Market.Settings.Configuration.FillLimit.Crop;
            transitResEnabled.Checked = vill.Market.Settings.Configuration.Enabled;
            TransitArrival.Text = vill.Market.Settings.Configuration.TransitArrival.ToString();
            LastTransit.Text = vill.Market.LastTransit.ToString();
            //Send res to main vill config
            woodSend.Value = vill.Market.Settings.Configuration.SendResLimit.Wood;
            claySend.Value = vill.Market.Settings.Configuration.SendResLimit.Clay;
            ironSend.Value = vill.Market.Settings.Configuration.SendResLimit.Iron;
            cropSend.Value = vill.Market.Settings.Configuration.SendResLimit.Crop;

            //For npc
            npcEnabled.Checked = vill.Market.Npc.Enabled;
            overflowProtection.Checked = vill.Market.Npc.NpcIfOverflow;
            numericUpDown4.Value = vill.Market.Npc.ResourcesRatio.Wood;
            numericUpDown3.Value = vill.Market.Npc.ResourcesRatio.Clay;
            numericUpDown2.Value = vill.Market.Npc.ResourcesRatio.Iron;
            numericUpDown1.Value = vill.Market.Npc.ResourcesRatio.Crop;
        }

        private void transitResEnabled_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.Enabled = transitResEnabled.Checked;

        private void overflowProtection_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Npc.NpcIfOverflow = overflowProtection.Checked;

        private void numericUpDown4_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Npc.ResourcesRatio.Wood = (long)numericUpDown4.Value;

        private void numericUpDown3_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Npc.ResourcesRatio.Clay = (long)numericUpDown3.Value;

        private void numericUpDown2_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Npc.ResourcesRatio.Iron = (long)numericUpDown2.Value;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Npc.ResourcesRatio.Crop = (long)numericUpDown1.Value;

        private void npcEnabled_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Npc.Enabled = npcEnabled.Checked;

        #region SendMainVill Callbacks
        private void woodSend_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.SendResLimit.Wood = (long)woodSend.Value;

        private void claySend_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.SendResLimit.Clay = (long)claySend.Value;

        private void ironSend_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.SendResLimit.Iron = (long)ironSend.Value;

        private void cropSend_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.SendResLimit.Crop = (long)cropSend.Value;
        #endregion

        #region TargetLimit Callbacks
        private void TargetLimitWood_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.TargetLimit.Wood = (long)TargetLimitWood.Value;

        private void TargetLimitClay_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.TargetLimit.Clay = (long)TargetLimitClay.Value;

        private void TargetLimitIron_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.TargetLimit.Iron = (long)TargetLimitIron.Value;

        private void TargetLimitCrop_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.TargetLimit.Crop = (long)TargetLimitCrop.Value;
        #endregion

        #region FillLimit Callbacks
        private void FillLimitWood_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.FillLimit.Wood = (long)FillLimitWood.Value;

        private void FillLimitClay_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.FillLimit.Clay = (long)FillLimitClay.Value;

        private void FillLimitIron_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.FillLimit.Iron = (long)FillLimitIron.Value;

        private void FillLimitCrop_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().Market.Settings.Configuration.FillLimit.Crop = (long)FillLimitCrop.Value;
        #endregion

    }
}
