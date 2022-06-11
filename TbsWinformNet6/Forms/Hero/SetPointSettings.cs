using System;
using System.Windows.Forms;
using TbsCore.Models.AccModels;

namespace TbsWinformNet6.Forms.Hero
{
    public partial class SetPointSettings : Form
    {
        public byte Strength { get; private set; }
        public byte OffBonus { get; private set; }
        public byte DeffBonus { get; private set; }
        public byte Resources { get; private set; }

        public SetPointSettings(Account acc)
        {
            InitializeComponent();

            var heroUpgrade = acc.Hero.Settings.Upgrades;
            strengthNumberBox.Value = 0;
            offBonusNumberBox.Value = 0;
            deffBonusNumberBox.Value = 0;
            resourcesNumberBox.Value = 0;
            strengthNumberBox.Value = heroUpgrade[0];
            offBonusNumberBox.Value = heroUpgrade[1];
            deffBonusNumberBox.Value = heroUpgrade[2];
            resourcesNumberBox.Value = heroUpgrade[3];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Strength = (byte)strengthNumberBox.Value;
            OffBonus = (byte)offBonusNumberBox.Value;
            DeffBonus = (byte)deffBonusNumberBox.Value;
            Resources = (byte)resourcesNumberBox.Value;
            Close();
        }

        private void strength_ValueChanged(object sender, EventArgs e)
        {
            button1.Enabled = strengthNumberBox.Value + offBonusNumberBox.Value + deffBonusNumberBox.Value + resourcesNumberBox.Value == 4;
        }

        private void offBonus_ValueChanged(object sender, EventArgs e)
        {
            button1.Enabled = strengthNumberBox.Value + offBonusNumberBox.Value + deffBonusNumberBox.Value + resourcesNumberBox.Value == 4;
        }

        private void deffBonus_ValueChanged(object sender, EventArgs e)
        {
            button1.Enabled = strengthNumberBox.Value + offBonusNumberBox.Value + deffBonusNumberBox.Value + resourcesNumberBox.Value == 4;
        }

        private void resources_ValueChanged(object sender, EventArgs e)
        {
            button1.Enabled = strengthNumberBox.Value + offBonusNumberBox.Value + deffBonusNumberBox.Value + resourcesNumberBox.Value == 4;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}