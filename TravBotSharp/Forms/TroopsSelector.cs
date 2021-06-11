using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravBotSharp.Forms
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