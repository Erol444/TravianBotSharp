using System;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Forms
{
    public partial class TroopsSelector : Form
    {
        private bool hero;
        private int[] troops;

        public TroopsSelector(Classificator.TribeEnum tribeEnum)
        {
            InitializeComponent();

            troopsSelectorUc1.Init(tribeEnum);
        }

        public int[] Troops
        {
            get => troops;
            set
            {
                troops = value;
                troopsSelectorUc1.Troops = troops;
            }
        }

        public bool Hero
        {
            get => hero;
            set
            {
                hero = value;
                troopsSelectorUc1.Hero = hero;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}