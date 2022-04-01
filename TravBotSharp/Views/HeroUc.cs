using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;
using TravBotSharp.Interfaces;
using TravBotSharp.Forms.Hero;

namespace TravBotSharp.Views
{
    public partial class HeroUc : TbsBaseUc, ITbsUc
    {
        public HeroUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            buyAdventuresCheckBox.Checked = acc.Hero.Settings.BuyAdventures;
            checkBoxAutoSendToAdventures.Checked = acc.Hero.Settings.AutoSendToAdventure;
            autoReviveHero.Checked = acc.Hero.Settings.AutoReviveHero;
            refreshInfo.Checked = acc.Hero.Settings.AutoRefreshInfo;
            helmetSwitcher.Checked = acc.Hero.Settings.AutoSwitchHelmets;
            autoEquip.Checked = acc.Hero.Settings.AutoEquip;

            autoSetHeroPoints.Checked = acc.Hero.Settings.AutoSetPoints;
            checkBox1.Checked = acc.Hero.Settings.AutoAuction;

            if (acc.Hero.Items == null) return;

            heroItemsList.Items.Clear();
            if (acc.Hero.Items.Count > 0)
            {
                foreach (var item in acc.Hero.Items)
                {
                    var viewItem = new ListViewItem();

                    var attr = item.Item.ToString().Split('_');

                    viewItem.SubItems[0].Text = attr[0];
                    viewItem.SubItems.Add(attr[1]);
                    viewItem.SubItems.Add(attr[2] == "0" ? "" : attr[2]);
                    viewItem.SubItems.Add(item.Count.ToString());

                    heroItemsList.Items.Add(viewItem);
                }
            }

            equiptList.Items.Clear();
            if (acc.Hero.Equipt == null)
            {
                acc.Hero.Equipt = new System.Collections.Generic.Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum>();
            }
            foreach (var pair in acc.Hero.Equipt)
            {
                var viewItem = new ListViewItem();

                var attr = pair.Value.ToString().Split('_');

                viewItem.SubItems[0].Text = attr[0];
                viewItem.SubItems.Add(attr[1]);
                viewItem.SubItems.Add(attr[2] == "0" ? "" : attr[2]);

                equiptList.Items.Add(viewItem);
            }

            // Update hero info
            string heroInfoStr = $"Health: {acc.Hero.HeroInfo.Health}\n";
            heroInfoStr += $"Hero Status: {acc.Hero.Status}\n";
            heroInfoStr += $"Hero home village: {HeroHelper.GetHeroHomeVillage(acc)?.Name ?? "UNKNOWN"}\n";
            heroInfoStr += $"Hero Arrival: {acc.Hero.HeroArrival}\n";
            heroInfoStr += $"Level: {acc.Hero.HeroInfo.Level}\n";
            heroInfoStr += $"Experience: {acc.Hero.HeroInfo.Experience}\n";
            heroInfoStr += $"Attack points: {acc.Hero.HeroInfo.FightingStrengthPoints}\n";
            heroInfoStr += $"Off Bonus points: {acc.Hero.HeroInfo.OffBonusPoints}\n";
            heroInfoStr += $"Deff Bonus points: {acc.Hero.HeroInfo.DeffBonusPoints}\n";
            heroInfoStr += $"Resources points: {acc.Hero.HeroInfo.ResourcesPoints}\n";
            heroInfoStr += $"Number of adventures: {acc.Hero.AdventureNum}";
            heroInfo.Text = heroInfoStr;

            //Adventures
            var advStr = new List<string>();
            foreach (var adv in acc.Hero.Adventures)
            {
                advStr.Add(adv.Coordinates.ToString() + " - " + adv.Difficulty.ToString() + " adventure");
            }
            adventures.Text = string.Join("\n", advStr);
        }

        private void checkBoxAutoSendToAdventures_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Hero.Settings.AutoSendToAdventure = checkBoxAutoSendToAdventures.Checked;
        }

        private void buyAdventuresCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Hero.Settings.BuyAdventures = buyAdventuresCheckBox.Checked;
        }

        private void autoSetHeroPoints_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.AutoSetPoints = autoSetHeroPoints.Checked;

        private void autoReviveHero_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.AutoReviveHero = autoReviveHero.Checked;

        private void refreshInfo_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.AutoRefreshInfo = refreshInfo.Checked;

        private void autoEquip_CheckedChanged(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            acc.Hero.Settings.AutoEquip = autoEquip.Checked;
            if (autoEquip.Checked) TurnOnAutoRefresh(acc);
        }

        /// <summary>
        /// If you want to use Auto-use res or Auto-Equip hero, you need to auto-refresh hero info
        /// </summary>
        /// <param name="acc"></param>
        private void TurnOnAutoRefresh(Account acc)
        {
            acc.Hero.Settings.AutoRefreshInfo = true;
            refreshInfo.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().Tasks.Add(new HeroUpdateInfo() { ExecuteAt = DateTime.Now });
        }

        private void button2_Click(object sender, EventArgs e) // Update adventures
        {
            var acc = GetSelectedAcc();
            acc.Tasks.Add(new StartAdventure()
            {
                ExecuteAt = DateTime.Now,
                UpdateOnly = true
            });
        }

        private void helmetSwitcher_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.AutoSwitchHelmets = helmetSwitcher.Checked;

        private void button3_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            using (var form = new AdventuresSettings(acc))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    acc.Hero.Settings.MinHealth = form.MinHealth;
                    acc.Hero.Settings.MaxDistance = form.MaxDistance;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            using (var form = new ReviveSettings(acc))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    acc.Hero.ReviveInVillage = form.VillageId;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            using (var form = new UpdateSetings(acc))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    acc.Hero.Settings.MaxUpdate = form.Min;
                    acc.Hero.Settings.MinUpdate = form.Max;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            using (var form = new SetPointSettings(acc))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    var heroUpgrade = acc.Hero.Settings.Upgrades;

                    heroUpgrade[0] = form.Strength;
                    heroUpgrade[1] = form.OffBonus;
                    heroUpgrade[2] = form.DeffBonus;
                    heroUpgrade[3] = form.Resources;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            acc.Hero.Settings.AutoAuction = checkBox1.Checked;
        }
    }
}