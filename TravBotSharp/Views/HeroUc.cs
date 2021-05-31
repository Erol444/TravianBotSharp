﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;
using TravBotSharp.Interfaces;

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
            minHeroHealthUpDown.Value = acc.Hero.Settings.MinHealth;
            autoReviveHero.Checked = acc.Hero.Settings.AutoReviveHero;
            refreshInfo.Checked = acc.Hero.Settings.AutoRefreshInfo;
            helmetSwitcher.Checked = acc.Hero.Settings.AutoSwitchHelmets;
            autoEquip.Checked = acc.Hero.Settings.AutoEquip;

            var heroUpgrade = acc.Hero.Settings.Upgrades;
            strength.Value = 0;
            offBonus.Value = 0;
            deffBonus.Value = 0;
            resources.Value = 0;
            strength.Value = heroUpgrade[0];
            offBonus.Value = heroUpgrade[1];
            deffBonus.Value = heroUpgrade[2];
            resources.Value = heroUpgrade[3];

            autoSetHeroPoints.Checked = acc.Hero.Settings.AutoSetPoints;
            maxDistanceUpDown.Value = acc.Hero.Settings.MaxDistance;
            LimitHeroPoints();

            maxInterval.Value = acc.Hero.Settings.MaxUpdate;
            minInterval.Value = acc.Hero.Settings.MinUpdate;

            SupplyResVillageComboBox.Items.Clear();
            foreach (var vill in acc.Villages)
            {
                SupplyResVillageComboBox.Items.Add(vill.Name);
            }
            if (SupplyResVillageComboBox.Items.Count > 0)
            {
                SupplyResVillageComboBox.SelectedIndex = 0;
                SupplyResVillageSelected.Text = "Selected: " + AccountHelper.GetHeroReviveVillage(acc).Name;
            }

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

        private void minHeroHealthUpDown_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Hero.Settings.MinHealth = (int)minHeroHealthUpDown.Value;
        }

        private void strength_ValueChanged(object sender, EventArgs e)
        {
            LimitHeroPoints();
        }

        private void offBonus_ValueChanged(object sender, EventArgs e)
        {
            LimitHeroPoints();
        }

        private void deffBonus_ValueChanged(object sender, EventArgs e)
        {
            LimitHeroPoints();
        }

        private void resources_ValueChanged(object sender, EventArgs e)
        {
            LimitHeroPoints();
        }

        private int HeroPointsUSer()
        {
            int str = (int)strength.Value;
            int off = (int)offBonus.Value;
            int deff = (int)deffBonus.Value;
            int res = (int)resources.Value;
            return str + off + deff + res;
        }

        private void LimitHeroPoints()
        {
            int lockPoints = HeroPointsUSer();
            strength.Maximum = strength.Value + 4 - lockPoints;
            offBonus.Maximum = offBonus.Value + 4 - lockPoints;
            deffBonus.Maximum = deffBonus.Value + 4 - lockPoints;
            resources.Maximum = resources.Value + 4 - lockPoints;
            var acc = GetSelectedAcc();
            var vals = new byte[] { (byte)strength.Value, (byte)offBonus.Value, (byte)deffBonus.Value, (byte)resources.Value };
            acc.Hero.Settings.Upgrades = vals;
        }

        private void autoSetHeroPoints_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.AutoSetPoints = autoSetHeroPoints.Checked;

        private void maxDistanceUpDown_ValueChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.MaxDistance = (int)maxDistanceUpDown.Value;

        private void autoReviveHero_CheckedChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.AutoReviveHero = autoReviveHero.Checked;

        private void SupplyResourcesButton_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = acc.Villages[SupplyResVillageComboBox.SelectedIndex];
            acc.Hero.ReviveInVillage = vill.Id;
            SupplyResVillageSelected.Text = "Selected: " + vill.Name;
        }

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

        private void minInterval_ValueChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.MinUpdate = (int)minInterval.Value;

        private void maxInterval_ValueChanged(object sender, EventArgs e) =>
            GetSelectedAcc().Hero.Settings.MaxUpdate = (int)maxInterval.Value;
    }
}