using System.Reflection;
using System.Timers;
using TbsCore.Database;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.Logging;
using TbsWinformNet6.Forms;
using TbsWinformNet6.Helpers;
using TbsWinformNet6.Interfaces;

namespace TbsWinformNet6
{
    public partial class ControlPanel : Form
    {
        private List<Account> accounts = new List<Account>();
        private int accSelected = 0;
        private System.Timers.Timer saveAccountsTimer;
        private ITbsUc[] Ucs;
        private bool closing;

        public ControlPanel()
        {
            closing = false;
            InitializeComponent();
            //read list of accounts!
            SerilogSingleton.Init();

            LoadAccounts();
            accListView.Select();

            // Be sure to have these in correct order!
            Ucs = new ITbsUc[]
            {
                generalUc1,
                heroUc1,
                villagesUc1,
                overviewUc1,
                overviewTroopsUc1,
                farmingUc1,
                newVillagesUc1,
                questsUc1,
                debugUc1,
            };

            // Initialize all the views
            foreach (var uc in Ucs) uc.Init(this);

            saveAccountsTimer = new System.Timers.Timer(1000 * 60 * 30); // Every 30 min
            saveAccountsTimer.Elapsed += SaveAccounts_TimerElapsed;
            saveAccountsTimer.AutoReset = true;
            saveAccountsTimer.Start();

            // So TbsCore can access forms and alert user
            IoHelperCore.AlertUser = IoHelperForms.AlertUser;

            checkNewVersion();
            this.debugUc1.InitLog(LogOutput.Instance);
            UseragentDatabase.Instance.Load();
        }

        private void SaveAccounts_TimerElapsed(object sender, ElapsedEventArgs e) => IoHelperCore.SaveAccounts(accounts);

        private void LoadAccounts()
        {
            accounts = DbRepository.GetAccounts();

            accounts.ForEach(x =>
            {
                ObjectHelper.FixAccObj(x, x);
                x.Load();
                x.Tasks.OnUpdateTask = debugUc1.UpdateTaskTable;

                RefreshAccView();
            });
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
                    DbRepository.SaveAccount(acc);
                    if (string.IsNullOrEmpty(acc.AccInfo.Nickname) ||
                        string.IsNullOrEmpty(acc.AccInfo.ServerUrl)) return;

                    acc.Load();
                    acc.Tasks.OnUpdateTask = debugUc1.UpdateTaskTable;
                    accounts.Add(acc);
                    RefreshAccView();
                }
            }
        }

        private async void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closing) return;
            e.Cancel = true;
            closing = true;
            await Task.Yield();
            Form closingForm = new Closing();
            var formTask = Task.Run(() => Invoke(new Action(() => closingForm.ShowDialog(this))));
            var savingTask = Task.Run(async () =>
            {
                IoHelperCore.SaveAccounts(accounts);
                var tasks = new List<Task>();
                foreach (var acc in accounts)
                {
                    tasks.Add(IoHelperCore.Logout(acc));
                }

                await Task.WhenAll(tasks);

                foreach (var acc in accounts)
                {
                    acc.Dispose();
                }

                SerilogSingleton.Close();
            });

            await savingTask;
            var pluginsFolder = Path.Combine(AppContext.BaseDirectory, "Plugins");
            if (Directory.Exists(pluginsFolder))
            {
                Directory.Delete(pluginsFolder, true);
            }
            closingForm.Close();
            await formTask;
            closingForm.Dispose();

            Close();
        }

        /// <summary>
        /// Refreshes the account view. Account currently selected will be colored in blue.
        /// </summary>
        public void RefreshAccView()
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
            item.SubItems[0].Text = $"{nick}"; //account
            item.SubItems[0].ForeColor = Color.FromName(selected ? "DodgerBlue" : "Black");
            //item.SubItems.Add("❌"); //proxy error
            item.SubItems.Add(IoHelperCore.UrlRemoveHttp(url));
            accListView.Items.Add(item);
        }

        private void button2_Click(object sender, EventArgs e) //login button
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
            if (!acc.CanLogin())
            {
                _ = MessageBox.Show("There is error (Sorry for lack of infomation)", "Cannot login");
                return;
            }
            var thread = new Thread(async () =>
            {
                acc.Status = Status.Starting;
                generalUc1.UpdateBotRunning();
                button2.Invoke(new Action(() => button2.Enabled = false));
                var success = await IoHelperCore.Login(acc);
                if (success)
                {
                    acc.Status = Status.Online;
                }
                else
                {
                    _ = MessageBox.Show("Check debug log to more info", $"Error while logging {acc.AccInfo.Nickname}", MessageBoxButtons.OK);
                    acc.Status = Status.Offline;
                    button2.Invoke(new Action(() => button2.Enabled = true));
                }
                if (GetSelectedAcc() == acc)
                {
                    button5.Invoke(new Action(() => button5.Enabled = true));
                }
                generalUc1.UpdateBotRunning();
            });
            thread.Start();
        }

        private void button3_Click(object sender, EventArgs e) // Remove an account
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
            if (acc.Status != Status.Offline)
            {
                _ = MessageBox.Show("You need logout this account before remove it", "Cannot remove this account", MessageBoxButtons.OK);
                return;
            }
            IoHelperCore.RemoveCache(acc);
            accounts.Remove(acc);
            DbRepository.RemoveAccount(acc);
            accListView.Items.RemoveAt(accSelected);
            acc.Dispose();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFrontEnd();
        }

        private void accListView_SelectedIndexChanged(object sender, EventArgs e) // Different acc selected
        {
            // remove event task update on previous account
            if (GetSelectedAcc()?.Tasks != null) GetSelectedAcc().Tasks.OnUpdateTask = null;

            var indicies = accListView.SelectedIndices;
            if (indicies.Count > 0)
            {
                accSelected = indicies[0];
            }
            var acc = GetSelectedAcc();

            button2.Enabled = acc.CanLogin();
            button5.Enabled = acc.CanLogout();

            UpdateFrontEnd();

            foreach (ListViewItem item in accListView.Items)
            {
                item.SubItems[0].ForeColor = Color.FromName("Black");
            }
            accListView.Items[accSelected].SubItems[0].ForeColor = Color.FromName("DodgerBlue");

            if (acc.Tasks != null)
            {
                acc.Tasks.OnUpdateTask = debugUc1.UpdateTaskTable;
                debugUc1.UpdateTaskTable();
            }
        }

        private void UpdateFrontEnd()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;

            // Refresh data in this tab!
            Ucs.ElementAtOrDefault(accTabController.SelectedIndex)?.UpdateUc();
        }

        private void button7_Click(object sender, EventArgs e) // Edit an account
        {
            var acc = GetSelectedAcc();
            if (acc != null)
            {
                using (var form = new AddAccount(acc))
                {
                    form.UpdateWindow();
                    DbRepository.SaveAccount(acc);
                    var result = form.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        // TODO: log
                    }
                }
            }
        }

        public Account GetSelectedAcc()
        {
            try
            {
                if (accounts.Count <= accSelected) return accounts.FirstOrDefault();
                return accounts[accSelected];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void button5_Click(object sender, EventArgs e) // Logout
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
            if (!acc.CanLogout())
            {
                _ = MessageBox.Show("Please wait bot complete its task", "Cannot logout");
                return;
            }

            var thread = new Thread(async () =>
            {
                acc.Status = Status.Stopping;
                generalUc1.UpdateBotRunning();
                button5.Invoke(new Action(() => button5.Enabled = false));
                await IoHelperCore.Logout(GetSelectedAcc());

                acc.Status = Status.Offline;
                generalUc1.UpdateBotRunning();
                if (GetSelectedAcc() == acc)
                {
                    button2.Invoke(new Action(() => button2.Enabled = false));
                }
                acc.Logger.Information("Account logged out");
            });
            thread.Start();
        }

        private void button6_Click(object sender, EventArgs e) // Login all accounts
        {
            var thread = new Thread(async () =>
            {
                var ran = new Random();
                var failed = new List<string>();
                await Task.Run(async () =>
                {
                    foreach (var acc in accounts)
                    {
                        if (!acc.CanLogin()) continue;
                        acc.Status = Status.Starting;

                        if (GetSelectedAcc() == acc) generalUc1.UpdateBotRunning();

                        var result = await IoHelperCore.Login(acc);
                        if (!result)
                        {
                            failed.Add(acc.AccInfo.Nickname);
                            acc.Status = Status.Offline;
                        }
                        else
                        {
                            await Task.Delay(ran.Next(10000, 30000));
                            acc.Status = Status.Online;
                        }
                        if (GetSelectedAcc() == acc) generalUc1.UpdateBotRunning();
                    }
                });

                if (failed.Count > 0)
                {
                    _ = MessageBox.Show($"Error while logging {string.Join(" & ", failed)}", "Check debug log each account to more info", MessageBoxButtons.OK);
                }
                else
                {
                    _ = MessageBox.Show($"Complete login all account in TBS's account list", "Success login all account ", MessageBoxButtons.OK);
                }
            });
            thread.Start();
        }

        private async void button4_Click(object sender, EventArgs e) // Logout all accounts
        {
            var tasks = new List<Task>();
            foreach (var acc in accounts)
            {
                tasks.Add(Task.Run(async () =>
               {
                   acc.Status = Status.Stopping;
                   if (GetSelectedAcc() == acc) generalUc1.UpdateBotRunning();
                   await IoHelperCore.Logout(acc);
                   acc.Status = Status.Offline;
                   if (GetSelectedAcc() == acc) generalUc1.UpdateBotRunning();
               }));
            }

            await Task.WhenAll(tasks);
            _ = MessageBox.Show($"Complete logout account in TBS's account list", "Logout all account", MessageBoxButtons.OK);
        }

        private void checkNewVersion()
        {
            new Thread(async () =>
            {
                var result = await Task.WhenAll(GithubHelper.CheckGitHubLatestVersion(), GithubHelper.CheckGitHublatestBuild());
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                currentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);
                var isNewAvailable = result[0] != null && (currentVersion.CompareTo(result[0]) < 0);

                if (isNewAvailable)
                {
                    using (var form = new NewRelease())
                    {
                        form.IsNewVersion = isNewAvailable;

                        form.CurrentVersion = currentVersion;
                        form.LatestVersion = result[0] ?? currentVersion;
                        form.LatestBuild = result[1] ?? currentVersion;

                        _ = form.ShowDialog();
                    }
                }
            }).Start();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (var form = new AddAccounts())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var accList = form.GetAccounts();
                    foreach (var acc in accList)
                    {
                        DbRepository.SaveAccount(acc);
                        if (string.IsNullOrEmpty(acc.AccInfo.Nickname) ||
                            string.IsNullOrEmpty(acc.AccInfo.ServerUrl)) return;

                        acc.Load();
                        accounts.Add(acc);
                    }

                    RefreshAccView();
                }
            }
        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.WindowState = FormWindowState.Normal;
            this.Focus();
            this.Show();
        }
    }
}