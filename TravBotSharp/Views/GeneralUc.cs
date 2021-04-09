using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TbsCore.Models.Settings;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Interfaces;
using TravBotSharp.Forms;

namespace TravBotSharp.Views
{
    public partial class GeneralUc : TbsBaseUc, ITbsUc
    {
        private readonly string[] allyBonus = new string[] { "Recruitment", "Philosophy", "Metallurgy", "Commerce" };
        private int bonusSelected = 0, resPrioSel = 0;

        public GeneralUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;

            SupplyResVillageComboBox.Items.Clear();
            foreach (var vill in acc.Villages)
            {
                SupplyResVillageComboBox.Items.Add(vill.Name);
            }
            if (SupplyResVillageComboBox.Items.Count > 0)
            {
                SupplyResVillageComboBox.SelectedIndex = 0;
                SupplyResVillageSelected.Text = "Selected: " + AccountHelper.GetMainVillage(acc).Name;
            }

            fillInAdvanceUpDown.Value = acc.Settings.FillInAdvance;
            FillForUpDown.Value = acc.Settings.FillFor;

            autoReadIGMs.Checked = acc.Settings.AutoReadIgms;
            autoRandomTasks.Checked = acc.Settings.AutoRandomTasks;
            extendProtection.Checked = acc.Settings.ExtendProtection;

            watchAdsUpDown.Value = acc.Settings.WatchAdAbove;

            disableImagesCheckbox.Checked = acc.Settings.DisableImages;
            headlessCheckbox.Checked = acc.Settings.HeadlessMode;
            reopenChrome.Checked = acc.Settings.AutoCloseDriver;
            openMinimizedCheckbox.Checked = acc.Settings.OpenMinimized;

            donateAbove.Value = acc.Settings.DonateAbove;
            donateExcessOf.Value = acc.Settings.DonateExcessOf;

            sleepMax.Value = acc.Settings.Time.MaxSleep;
            sleepMin.Value = acc.Settings.Time.MinSleep;
            workMax.Value = acc.Settings.Time.MaxWork;
            workMin.Value = acc.Settings.Time.MinWork;
            UpdateBotRunning();
            UpdaterBonusPrio(acc);
            UpdaterResPrio(acc);
        }

        private void SupplyResourcesButton_Click(object sender, EventArgs e) //select village to supply res to new villages
        {
            var acc = GetSelectedAcc();
            var vill = acc.Villages[SupplyResVillageComboBox.SelectedIndex];
            acc.Settings.MainVillage = vill.Id;
            SupplyResVillageSelected.Text = "Selected: " + vill.Name;
        }

        private void button21_Click(object sender, EventArgs e) //start UNL server tasks
        {
            var acc = GetSelectedAcc();
            int sec = 1;
            TaskExecutor.AddTask(acc, new TTWarsGetRes() { ExecuteAt = DateTime.Now.AddSeconds(sec) });
            TaskExecutor.AddTask(acc, new TrainExchangeRes() { ExecuteAt = DateTime.Now.AddSeconds(sec + 5), troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero });
            TaskExecutor.AddTask(acc, new TrainTroops()
            {
                ExecuteAt = DateTime.Now.AddSeconds(sec + 11),
                Troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero,
                HighSpeedServer = true
            });
            TaskExecutor.AddTask(acc, new TTWarsGetAnimals() { ExecuteAt = DateTime.Now.AddSeconds(sec + 33) });
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) //auto +25% and plus acc activator
        {
            GetSelectedAcc().Settings.AutoActivateProductionBoost = checkBox4.Checked;
        }

        private void startTimersButton_Click(object sender, EventArgs e) //start timer
        {
            var acc = GetSelectedAcc();
            acc.TaskTimer.Start();

            acc.Tasks.Clear();
            AccountHelper.StartAccountTasks(acc);
            acc.Villages.ForEach(x => x.UnfinishedTasks.Clear());
            UpdateBotRunning();
        }

        private void button16_Click(object sender, EventArgs e) //all villages farm tasks
        {
            var acc = GetSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.FarmVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button17_Click(object sender, EventArgs e) //all villages support tasks
        {
            var acc = GetSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.SupplyVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button14_Click(object sender, EventArgs e) //all villages deff tasks
        {
            var acc = GetSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.DeffVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button4_Click(object sender, EventArgs e) //all villages off tasks
        {
            var acc = GetSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.OffVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button5_Click(object sender, EventArgs e) //all villages select from file
        {
            var acc = GetSelectedAcc();

            string location = IoHelperForms.PromptUserForBuidTasksLocation();

            if (location == null) return;

            foreach (var vill in acc.Villages)
            {
                IoHelperCore.AddBuildTasksFromFile(acc, vill, location);
            }
        }

        private void button18_Click(object sender, EventArgs e) //clear all villages build tasks
        {
            var acc = GetSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                vill.Build.Tasks.Clear();
            }
        }

        /// <summary>
        /// Automatically expand storage. For TTwars UNL/VIP servers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var expandTimes = (int)numericUpDown4.Value;
            var seconds = (int)numericUpDown5.Value;
            numericUpDown4.Value = 0;
            numericUpDown5.Value = 0;
            if (expandTimes != 0)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new TTWarsExpandStorage() { ExecuteAt = DateTime.Now, Times = expandTimes });
            }
            else if (seconds != 0)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new TTWarsExpandStorage() { ExecuteAt = DateTime.Now, Seconds = seconds });
            }
        }

        private void button3_Click(object sender, EventArgs e) //get only resources on UNL TTwars servers
        {
            var acc = GetSelectedAcc();
            int sec = 1;
            TaskExecutor.AddTask(acc, new TTWarsGetRes() { ExecuteAt = DateTime.Now.AddSeconds(sec) });
            TaskExecutor.AddTask(acc, new TrainExchangeRes() { ExecuteAt = DateTime.Now.AddSeconds(sec + 5), troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero });
            TaskExecutor.AddTask(acc, new TrainTroops() { ExecuteAt = DateTime.Now.AddSeconds(sec + 11), Troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero });
        }

        private void fillInAdvanceUpDown_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.FillInAdvance = (int)fillInAdvanceUpDown.Value;
        }

        private void FillForUpDown_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.FillFor = (int)FillForUpDown.Value;
        }

        private void autoReadIGMs_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.AutoReadIgms = autoReadIGMs.Checked;
        }

        private void button6_Click(object sender, EventArgs e) // Stop timers
        {
            GetSelectedAcc().TaskTimer?.Stop();
            UpdateBotRunning();
        }

        private void headlessCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.HeadlessMode = headlessCheckbox.Checked;
        }

        private void disableImagesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.DisableImages = disableImagesCheckbox.Checked;
        }

        private void workMin_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)workMin.Value;
            if (val > (int)workMax.Value)
            {
                workMin.Value = workMax.Value;
            }
            GetSelectedAcc().Settings.Time.MinWork = (int)workMin.Value;
        }

        private void workMax_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)workMax.Value;
            if (val < (int)workMin.Value)
            {
                workMax.Value = workMin.Value;
            }
            GetSelectedAcc().Settings.Time.MaxWork = (int)workMax.Value;
        }

        private void sleepMin_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)sleepMin.Value;
            if (val > (int)sleepMax.Value)
            {
                sleepMin.Value = sleepMax.Value;
            }
            GetSelectedAcc().Settings.Time.MinSleep = (int)sleepMin.Value;
        }

        private void sleepMax_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)sleepMax.Value;
            if (val < (int)sleepMin.Value)
            {
                sleepMax.Value = sleepMin.Value;
            }
            GetSelectedAcc().Settings.Time.MaxSleep = (int)sleepMax.Value;
        }

        private void reopenChrome_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.AutoCloseDriver = reopenChrome.Checked;
        }

        private void autoRandomTasks_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.AutoRandomTasks = autoRandomTasks.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().TaskTimer?.Start();
            UpdateBotRunning();
        }

        public void UpdateBotRunning(string running = null)
        {
            if (string.IsNullOrEmpty(running)) running = GetSelectedAcc()?.TaskTimer?.IsBotRunning()?.ToString();
            botRunning.Text = "Bot running: " + (string.IsNullOrEmpty(running) ? "false" : running);
        }

        private void openMinimizedCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.OpenMinimized = openMinimizedCheckbox.Checked;
        }

        private void watchAdsUpDown_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.WatchAdAbove = (int)watchAdsUpDown.Value;
        }

        private void extendProtection_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.ExtendProtection = extendProtection.Checked;
        }

        private void button8_Click(object sender, EventArgs e) => MoveBonusPrio(false); // Move bonus prio down

        private void button7_Click(object sender, EventArgs e) => MoveBonusPrio(true); // Move bonus prio up

        private void MoveBonusPrio(bool up)
        {
            if ((bonusSelected == 0 && up) || (bonusSelected == 3 && !up)) return;

            var acc = GetSelectedAcc();
            var curVal = acc.Settings.BonusPriority[bonusSelected];
            var nextIndex = up ? -1 : 1;
            acc.Settings.BonusPriority[bonusSelected] = acc.Settings.BonusPriority[bonusSelected + nextIndex];
            acc.Settings.BonusPriority[bonusSelected + nextIndex] = curVal;
            bonusSelected += nextIndex;
            UpdaterBonusPrio(acc);
        }

        private void UpdaterBonusPrio(Account acc)
        {
            if (acc.Settings.BonusPriority == null) acc.Settings.BonusPriority = new byte[4] { 0, 1, 2, 3 };
            priorityList.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                var item = new ListViewItem();
                item.Text = allyBonus[acc.Settings.BonusPriority[i]];
                item.ForeColor = Color.FromName(bonusSelected == i ? "DodgerBlue" : "Black");
                priorityList.Items.Add(item);
            }
        }

        private void priorityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bonusSelected = priorityList.SelectedItems[0].Index;
            if (bonusSelected < 0 || 3 < bonusSelected) bonusSelected = 0;
            UpdaterBonusPrio(GetSelectedAcc());
        }

        private void donateAbove_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.DonateAbove = (int)donateAbove.Value;
        }

        private void donateExcessOf_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().Settings.DonateExcessOf = (int)donateExcessOf.Value;
        }

        private void button9_Click(object sender, EventArgs e) // Change account access
        {
            TaskExecutor.AddTaskIfNotExists(GetSelectedAcc(), new ChangeAccess()
            {
                ExecuteAt = DateTime.Now,
                WaitSecMin = 0,
                WaitSecMax = 1
            });
        }

        private void button11_Click(object sender, EventArgs e) => MoveResPrio(true);

        private void button10_Click(object sender, EventArgs e) => MoveResPrio(false);

        private void MoveResPrio(bool up)
        {
            if ((resPrioSel == 0 && up) || (resPrioSel == 2 && !up)) return;

            var acc = GetSelectedAcc();
            var curVal = acc.Settings.ResSpendingPriority[resPrioSel];
            var nextIndex = up ? -1 : 1;
            acc.Settings.ResSpendingPriority[resPrioSel] = acc.Settings.ResSpendingPriority[resPrioSel + nextIndex];
            acc.Settings.ResSpendingPriority[resPrioSel + nextIndex] = curVal;
            resPrioSel += nextIndex;
            UpdaterResPrio(acc);
        }

        private void resPrioView_SelectedIndexChanged(object sender, EventArgs e)
        {
            resPrioSel = resPrioView.SelectedItems[0].Index;
            if (resPrioSel < 0 || 2 < resPrioSel) resPrioSel = 0;
            UpdaterResPrio(GetSelectedAcc());
        }

        /// <summary>
        /// Check new version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            new Thread(async () =>
            {
                var result = await Task.WhenAll(GithubHelper.CheckGitHubLatestVersion(), GithubHelper.CheckGitHublatestBuild());
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                currentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);
                var isNewAvailable = result[0] != null && (currentVersion.CompareTo(result[0]) < 0);

                using (var form = new NewRelease())
                {
                    form.IsNewVersion = isNewAvailable;

                    form.LatestVersion = result[0] ?? currentVersion;
                    form.LatestBuild = result[1] ?? currentVersion;
                    form.CurrentVersion = currentVersion;

                    _ = form.ShowDialog();
                }
            }).Start();
        }

        private void UpdaterResPrio(Account acc)
        {
            if (acc.Settings.ResSpendingPriority == null) acc.Settings.ResSpendingPriority = new ResSpendTypeEnum[3] {
                ResSpendTypeEnum.Celebrations,
                ResSpendTypeEnum.Building,
                ResSpendTypeEnum.Troops
            };

            resPrioView.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                var item = new ListViewItem();
                item.Text = acc.Settings.ResSpendingPriority[i].ToString();
                item.ForeColor = Color.FromName(resPrioSel == i ? "DodgerBlue" : "Black");
                resPrioView.Items.Add(item);
            }
        }
    }
}