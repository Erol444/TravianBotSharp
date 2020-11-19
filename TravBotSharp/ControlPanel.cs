using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            RefreshAccView();
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
                    RefreshAccView();
                }
            }
       }

        private void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            IoHelperCore.Quit(accounts);
        }

        /// <summary>
        /// Refreshes the account view. Account currently selected will be colored in blue.
        /// </summary>
        private void RefreshAccView()
        {
            accListView.Items.Clear();
            for (int i = 0; i < accounts.Count; i++)
            {
                var access = accounts[i].Access.GetCurrentAccess();
                InsertAccIntoListView(accounts[i].AccInfo.Nickname,
                    accounts[i].AccInfo.ServerUrl,
                    access?.Proxy ?? "NO ACCESS",
                    access?.ProxyPort ?? 0,
                    i == accSelected);
            }
        }
        private void InsertAccIntoListView(string nick, string url, string proxy, int port, bool selected = false)
        {
            var item = new ListViewItem();
            item.SubItems[0].Text = $"{nick} ({IoHelperCore.UrlRemoveHttp(url)})"; //account
            item.SubItems[0].ForeColor = Color.FromName(selected ? "DodgerBlue" : "Black");
            //item.SubItems.Add("❌"); //proxy error
            item.SubItems.Add(string.IsNullOrEmpty(proxy) ? "/" : proxy + ":" + port); //proxy
            accListView.Items.Add(item);
        }

        private async void button2_Click(object sender, EventArgs e) //login button
        {
            var acc = GetSelectedAcc();
            if (acc.Access.AllAccess.Count > 0)
            {
                new Thread(() => _ = IoHelperCore.LoginAccount(acc)).Start();
                generalUc1.UpdateBotRunning("true");
                return;
            }

            // Alert user that account has no access defined
            string message = "Account you are trying to login has no access' defined. Please edit the account.";
            string caption = "Error in account";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
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
            RefreshAccView();
        }
        private void UpdateFrontEnd()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
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
                    overviewUc1.UpdateTab();
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
                    debugUc1.Focus();
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
                for (int i = 0; i < acc.Villages.Count; i++) // Update villages list
                {
                    var item = new ListViewItem();
                    item.SubItems[0].Text = acc.Villages[i].Name;
                    item.SubItems[0].ForeColor = Color.FromName(villSelected == i ? "DodgerBlue" : "Black");
                    item.SubItems.Add(acc.Villages[i].Coordinates.x + "/" + acc.Villages[i].Coordinates.y); //coords
                    item.SubItems.Add(VillageHelper.VillageType(acc.Villages[i])); //type (resource)
                    item.SubItems.Add(VillageHelper.ResourceIndicator(acc.Villages[i])); //resources count
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
                    troopsUc1.UpdateTab();
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
            UpdateVillageTab(true);

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
            // Some error. Refresh acc list view, maybe this will help.
            if (villSelected >= acc.Villages.Count)
            {
                RefreshAccView();
                return null;
            }
            return acc.Villages[villSelected];
        }

        private void RefreshVill_Click(object sender, EventArgs e) // Refresh selected village
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            RefreshVillage(acc, vill);
        }
        private void RefreshVillage(Account acc, Village vill) // Refresh village
        {
            TaskExecutor.AddTask(acc, new UpdateVillage() { 
                ExecuteAt = DateTime.Now.AddHours(-1),
                Vill = vill,
                ImportTasks = false 
            });
        }

        private void RefreshAllVills_Click(object sender, EventArgs e) // Refresh all villages
        {
            var acc = GetSelectedAcc();
            acc.Villages.ForEach(x => RefreshVillage(acc, x));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //var acc = GetSelectedAcc();
            //TaskExecutor.AddTaskIfNotExists(acc, new FindVillageToSettle()
            //{
            //    Vill = AccountHelper.GetMainVillage(acc),
            //    ExecuteAt = DateTime.MinValue.AddHours(10)
            //});
        }

        private void button5_Click(object sender, EventArgs e) // Logout
        {
            new Thread(() => IoHelperCore.Logout(GetSelectedAcc())).Start();
            generalUc1.UpdateBotRunning("false");
        }

        private void button6_Click(object sender, EventArgs e) // Login all accounts
        {
            new Thread(async () =>
            {
                var ran = new Random();
                foreach (var acc in accounts)
                {
                    // If account is already running, don't login
                    if (acc.TaskTimer?.IsBotRunning() ?? false) continue;

                    _ = IoHelperCore.LoginAccount(acc);
                    await Task.Delay(ran.Next(500, 5000));
                }
            }).Start();
            generalUc1.UpdateBotRunning("true");
        }
    }
}