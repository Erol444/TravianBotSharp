using System;
using System.Windows.Forms;

namespace TbsWinformNet6.Forms
{
    public partial class TroopsSelector : Form
    {
        private int[] troops;
        private bool hero;

        public int[] Troops
        {
            get
            {
                return troops;
            }
            set
            {
                troops = value;
                troopsSelectorUc1.Troops = troops;
            }
        }

        public bool Hero
        {
            get
            {
                return hero;
            }
            set
            {
                hero = value;
                troopsSelectorUc1.Hero = hero;
            }
        }

        public TroopsSelector(TbsCore.Helpers.Classificator.TribeEnum tribeEnum)
        {
            InitializeComponent();

            troopsSelectorUc1.Init(tribeEnum);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}