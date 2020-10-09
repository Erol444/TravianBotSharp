using System;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Views
{
    public partial class QuestsUc : UserControl
    {
        ControlPanel main;
        public QuestsUc()
        {
            InitializeComponent();
        }
        public void UpdateTab()
        {
            var acc = getSelectedAcc();

            claimDaily.Checked = acc.Quests.ClaimDailyQuests;
            claimBeginner.Checked = acc.Quests.ClaimBeginnerQuests;

            claimVill.Items.Clear();
            foreach (var vill in acc.Villages)
            {
                claimVill.Items.Add(vill.Name);
            }
            if (claimVill.Items.Count > 0)
            {
                claimVill.SelectedIndex = 0;
                claimVillLabel.Text = "Selected: " + AccountHelper.GetQuestsClaimVillage(acc).Name;
            }
        }
        public void Init(ControlPanel _main)
        {
            main = _main;
        }
        private Account getSelectedAcc()
        {
            return main != null ? main.GetSelectedAcc() : null;
        }

        private void claimDailyQuests_CheckedChanged(object sender, EventArgs e) // Claim daily quests
        {
            getSelectedAcc().Quests.ClaimDailyQuests = claimDaily.Checked;
        }

        private void claimBeginnerQuests_CheckedChanged(object sender, EventArgs e) // Claim beginner quests
        {
            getSelectedAcc().Quests.ClaimBeginnerQuests = claimBeginner.Checked;
        }

        private void claimVillButton_Click(object sender, EventArgs e)
        {
            var acc = getSelectedAcc();
            var vill = acc.Villages[claimVill.SelectedIndex];
            acc.Quests.VillToClaim = vill.Id;
            claimVillLabel.Text = "Selected: " + vill.Name;
        }
    }
}
