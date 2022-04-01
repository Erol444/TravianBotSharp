using System;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Forms.Hero
{
    public partial class ReviveSettings : Form
    {
        public int VillageId { get; private set; }

        public ReviveSettings(Account acc)
        {
            InitializeComponent();
            SpawnVillages.Items.Clear();
            foreach (var vill in acc.Villages)
            {
                SpawnVillages.Items.Add(vill.Name);
            }
            if (SpawnVillages.Items.Count > 0)
            {
                var vill = acc.Villages.FirstOrDefault(x => x.Id == acc.Hero.ReviveInVillage);
                if (vill != null)
                {
                    SpawnVillages.SelectedIndex = acc.Villages.IndexOf(vill);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            VillageId = SpawnVillages.SelectedIndex;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}