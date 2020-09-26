using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp
{
    public partial class ControlPanel : Form
    {
        private List<Account> accounts = new List<Account>();
        private int accSelected = 0;
        private int villSelected = 0;
        public ControlPanel()
        {
            InitializeComponent();
            //read list of accounts!
            LoadAccounts();
            accListView.Select();

            generalUc1.Init(this);
            heroUc1.Init(this);
            buildUc1.Init(this);
            marketUc1.Init(this);
            troopsUc1.Init(this);
            overviewUc1.Init(this);
            farmingUc1.Init(this);
            newVillagesUc1.Init(this);
            deffendingUc1.Init(this);
            attackUc1.Init(this);
            debugUc1.Init(this);
            questsUc1.Init(this);
        }

        private void LoadAccounts()
        {
            accounts = IoHelperCore.ReadAccounts();
            foreach (var acc in accounts)
            {
                var access = acc.Access.GetCurrentAccess();
                InsertAccIntoListView(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl, access.Proxy, access.ProxyPort);
            }
        }

        private void button1_Click(object sender, EventArgs e) // Add account
        {
            using (var form = new AddAccount())
            {
                form.UpdateWindow();
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var acc = form.Acc;
                    if (string.IsNullOrEmpty(acc.AccInfo.Nickname) ||
                        string.IsNullOrEmpty(acc.AccInfo.ServerUrl)) return;

                    accounts.Add(acc);
                    var access = acc.Access.GetCurrentAccess();
                    InsertAccIntoListView(acc.AccInfo.Nickname,
                        acc.AccInfo.ServerUrl,
                        access.Proxy,
                        access.ProxyPort);
                }
            }
       }

        private void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            IoHelperCore.Quit(accounts);
        }

        private void InsertAccIntoListView(string nick, string url, string proxy, int port)
        {
            var item = new ListViewItem();
            item.SubItems[0].Text = $"{nick} ({IoHelperCore.UrlRemoveHttp(url)})"; //account
            item.SubItems.Add("❌"); //proxy error
            item.SubItems.Add(string.IsNullOrEmpty(proxy) ? "/" : proxy + ":" + port); //proxy
            accListView.Items.Add(item);
        }

        private async void button2_Click(object sender, EventArgs e) //login button
        {
            new Thread(() => _ = IoHelperCore.LoginAccount(GetSelectedAcc())).Start();
            generalUc1.UpdateBotRunning("true");
        }

        private void button3_Click(object sender, EventArgs e) // Remove an account
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;

            IoHelperCore.RemoveCache(acc);
            accounts.Remove(acc);
            accListView.Items.RemoveAt(accSelected);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFrontEnd();
        }

        private void accListView_SelectedIndexChanged(object sender, EventArgs e) // Different acc selected
        {
            var indicies = accListView.SelectedIndices;
            if (indicies.Count > 0)
            {
                accSelected = indicies[0];
            }
            var acc = GetSelectedAcc();
            // If account has no Wb object, it's not logged in at the moment
            button2.Enabled = acc != null && acc.Wb == null;

            UpdateFrontEnd();
        }
        private void UpdateFrontEnd()
        {
            if (GetSelectedAcc() == null) return;
            //refresh data in this tab!
            switch (accTabController.SelectedIndex) // AccTabController
            {
                case 0: // General
                    generalUc1.UpdateGeneralTab();
                    break;
                case 1: // Hero
                    heroUc1.UpdateTab();
                    break;
                case 2: // Villages
                    UpdateVillageTab();
                    break;
                case 3: // Overview
                    overviewUc1.UpdateOverviewTab();
                    break;
                case 4: // Farming
                    farmingUc1.UpdateTab();
                    break;
                case 5: // New villages
                    newVillagesUc1.UpdateTab();
                    break;
                case 6: // Deffending
                    deffendingUc1.UpdateTab();
                    break;
                case 7: // Quests
                    questsUc1.UpdateTab();
                    break;
                case 8: // Debug tab
                    debugUc1.UpdateTab();
                    break;
                default: break;
            }
        }
        private void UpdateVillageTab(bool updateVillList = true)
        {
            if (accounts.Count == 0) return;
            var acc = GetSelectedAcc();
            if (updateVillList)
            {
                VillagesListView.Items.Clear();
                foreach (var vill in acc.Villages) //update villages list
                {
                    var item = new ListViewItem();
                    item.SubItems[0].Text = vill.Name;
                    item.SubItems.Add(vill.Coordinates.x + "/" + vill.Coordinates.y); //coords
                    item.SubItems.Add(VillageHelper.VillageType(vill)); //type (resource)
                    item.SubItems.Add(VillageHelper.ResourceIndicator(vill)); //resources count
                    VillagesListView.Items.Add(item);
                }
            }

            if (acc.Villages.Count <= 0) return;
            switch (villageTabController.SelectedIndex)
            {
                case 0: // Build
                    buildUc1.UpdateBuildTab();
                    break;
                case 1: // Market
                    marketUc1.UpdateMarketTab();
                    break;
                case 2: // Troops
                    troopsUc1.UpdateTroopTab();
                    break;
                case 3: // Attack tab
                    attackUc1.UpdateTab();
                    break;
                default: break;
            }
        }

        private void villageTabController_SelectedIndexChanged(object sender, EventArgs e) //villageTabController tab changed event
        {
            UpdateVillageTab();
        }

        private void button7_Click(object sender, EventArgs e) // Edit an account
        {
            var acc = GetSelectedAcc();
            if (acc != null)
            {
                using (var form = new AddAccount(acc))
                {
                    form.UpdateWindow();
                    var result = form.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        // TODO: log
                    }
                }
            }
        }

        private void VillagesListView_SelectedIndexChanged(object sender, EventArgs e) //update building tab if its selected
        {
            var indicies = VillagesListView.SelectedIndices;
            if (indicies.Count > 0)
                villSelected = indicies[0];
            UpdateVillageTab(false);
        }

        public Account GetSelectedAcc()
        {
            try
            {
                if (accounts.Count <= accSelected) return accounts.FirstOrDefault();
                return accounts[accSelected];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Village GetSelectedVillage(Account acc = null)
        {
            if (acc == null) acc = GetSelectedAcc();
            //Some error. Reset acc list view, maybe this will help.
            if (villSelected >= acc.Villages.Count)
            {
                accListView.Items.Clear();
                foreach (var accInfo in accounts)
                {
                    var currentAccess = accInfo.Access.GetCurrentAccess();
                    InsertAccIntoListView(accInfo.AccInfo.Nickname, accInfo.AccInfo.ServerUrl, currentAccess.Proxy, currentAccess.ProxyPort);
                }
                return null;
            }
            return acc.Villages[villSelected];
        }

        private void RefreshVill_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            RefreshVillage(acc, vill);
        }
        private void RefreshVillage(Account acc, Village vill)
        {
            var executeAt = DateTime.Now.AddHours(-1);
            TaskExecutor.AddTask(acc, new UpdateDorf1() { ExecuteAt = executeAt, Vill = vill });
            TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = executeAt, Vill = vill });
            TaskExecutor.AddTask(acc, new UpdateTroops() { ExecuteAt = executeAt, Vill = vill });
            // Todo: refresh celebrities
        }

        private void RefreshAllVills_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            acc.Villages.ForEach(x => RefreshVillage(acc, x));
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e) // Logout
        {
            new Thread(() => IoHelperCore.Logout(GetSelectedAcc())).Start();
            generalUc1.UpdateBotRunning("false");
        }
    }
}