using System;
using System.Windows.Forms;
using TbsCore.Models.AccModels;

namespace TbsWinformNet6.Forms.Hero
{
    public partial class AdventuresSettings : Form
    {
        public int MinHealth { get; private set; }
        public int MaxDistance { get; private set; }

        public AdventuresSettings(Account acc)
        {
            InitializeComponent();
            minHeroHealthUpDown.Value = acc.Hero.Settings.MinHealth;
            maxDistanceUpDown.Value = acc.Hero.Settings.MaxDistance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            MinHealth = (int)minHeroHealthUpDown.Value;
            MaxDistance = (int)maxDistanceUpDown.Value;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}