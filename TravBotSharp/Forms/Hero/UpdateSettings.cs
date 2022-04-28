using System;
using System.Windows.Forms;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Forms.Hero
{
    public partial class UpdateSetings : Form
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public UpdateSetings(Account acc)
        {
            InitializeComponent();

            maxInterval.Value = acc.Hero.Settings.MaxUpdate;
            minInterval.Value = acc.Hero.Settings.MinUpdate;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Min = (int)minInterval.Value;
            Max = (int)maxInterval.Value;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void minInterval_ValueChanged(object sender, EventArgs e)
        {
            if (minInterval.Value > maxInterval.Value) minInterval.Value = maxInterval.Value;
        }

        private void maxInterval_ValueChanged(object sender, EventArgs e)
        {
            if (minInterval.Value > maxInterval.Value) maxInterval.Value = minInterval.Value;
        }
    }
}