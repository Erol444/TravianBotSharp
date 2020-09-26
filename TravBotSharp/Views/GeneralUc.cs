using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Views
{
    public partial class GeneralUc : UserControl
    {
        ControlPanel main;
        public void Init(ControlPanel _main)
        {
            main = _main;
            UpdateGeneralTab();
        }
        public GeneralUc()
        {
            InitializeComponent();
        }
        private Account getSelectedAcc()
        {
            return main.GetSelectedAcc();
        }
        public void UpdateGeneralTab()
        {
            var acc = getSelectedAcc();
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

            disableImagesCheckbox.Checked = acc.Settings.DisableImages;
            headlessCheckbox.Checked = acc.Settings.HeadlessMode;
            reopenChrome.Checked = acc.Settings.AutoCloseDriver;

            sleepMax.Value = acc.Settings.Time.MaxSleep;
            sleepMin.Value = acc.Settings.Time.MinSleep;
            workMax.Value = acc.Settings.Time.MaxWork;
            workMin.Value = acc.Settings.Time.MinWork;
            UpdateBotRunning();
        }

        private void SupplyResourcesButton_Click(object sender, EventArgs e) //select village to supply res to new villages
        {
            var acc = getSelectedAcc();
            var vill = acc.Villages[SupplyResVillageComboBox.SelectedIndex];
            acc.Settings.MainVillage = vill.Id;
            SupplyResVillageSelected.Text = "Selected: " + vill.Name;
        }


        private void button21_Click(object sender, EventArgs e) //start UNL server tasks
        {
            var acc = getSelectedAcc();
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
            getSelectedAcc().Settings.AutoActivateProductionBoost = checkBox4.Checked;
        }

        private void startTimersButton_Click(object sender, EventArgs e) //start timer
        {
            var acc = getSelectedAcc();
            acc.TaskTimer.Start();

            acc.Tasks.Clear();
            AccountHelper.StartAccountTasks(acc);
            UpdateBotRunning();
        }

        private void button16_Click(object sender, EventArgs e) //all villages farm tasks
        {
            var acc = getSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.FarmVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button17_Click(object sender, EventArgs e) //all villages support tasks
        {
            var acc = getSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.SupplyVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button14_Click(object sender, EventArgs e) //all villages deff tasks
        {
            var acc = getSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.DeffVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button4_Click(object sender, EventArgs e) //all villages off tasks
        {
            var acc = getSelectedAcc();
            foreach (var vill in acc.Villages)
            {
                DefaultConfigurations.OffVillagePlan(acc, vill);
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button5_Click(object sender, EventArgs e) //all villages select from file
        {
            var acc = getSelectedAcc();

            string location = IoHelperForms.PromptUserForBuidTasksLocation();

            List<BuildingTask> tasks;
            using (StreamReader sr = new StreamReader(location))
            {
                tasks = JsonConvert.DeserializeObject<List<BuildingTask>>(sr.ReadToEnd());
            }
            foreach (var vill in acc.Villages)
            {
                foreach (var task in tasks)
                {
                    BuildingHelper.AddBuildingTask(acc, vill, task);
                }
                BuildingHelper.RemoveCompletedTasks(vill, acc);
            }
        }

        private void button18_Click(object sender, EventArgs e) //clear all villages build tasks
        {
            var acc = getSelectedAcc();
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
            var acc = getSelectedAcc();
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
            var acc = getSelectedAcc();
            int sec = 1;
            TaskExecutor.AddTask(acc, new TTWarsGetRes() { ExecuteAt = DateTime.Now.AddSeconds(sec) });
            TaskExecutor.AddTask(acc, new TrainExchangeRes() { ExecuteAt = DateTime.Now.AddSeconds(sec + 5), troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero });
            TaskExecutor.AddTask(acc, new TrainTroops() { ExecuteAt = DateTime.Now.AddSeconds(sec + 11), Troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero });
        }

        private void fillInAdvanceUpDown_ValueChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.FillInAdvance = (int)fillInAdvanceUpDown.Value;
        }

        private void FillForUpDown_ValueChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.FillFor = (int)FillForUpDown.Value;
        }
        private void autoReadIGMs_CheckedChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.AutoReadIgms = autoReadIGMs.Checked;
        }

        private void button6_Click(object sender, EventArgs e) // Stop timers
        {
            getSelectedAcc().TaskTimer?.Stop();
            UpdateBotRunning();
        }

        private void headlessCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.HeadlessMode = headlessCheckbox.Checked;
        }

        private void disableImagesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.DisableImages = disableImagesCheckbox.Checked;
        }

        private void workMin_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)workMin.Value;
            if (val > (int)workMax.Value) {
                workMin.Value = workMax.Value;
            }
            getSelectedAcc().Settings.Time.MinWork = (int)workMin.Value;
        }

        private void workMax_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)workMax.Value;
            if (val < (int)workMin.Value)
            {
                workMax.Value = workMin.Value;
            }
            getSelectedAcc().Settings.Time.MaxWork = (int)workMax.Value;
        }

        private void sleepMin_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)sleepMin.Value;
            if (val > (int)sleepMax.Value)
            {
                sleepMin.Value = sleepMax.Value;
            }
            getSelectedAcc().Settings.Time.MinSleep = (int)sleepMin.Value;
        }

        private void sleepMax_ValueChanged(object sender, EventArgs e)
        {
            var val = (int)sleepMax.Value;
            if (val < (int)sleepMin.Value)
            {
                sleepMax.Value = sleepMin.Value;
            }
            getSelectedAcc().Settings.Time.MaxSleep = (int)sleepMax.Value;
        }

        private void reopenChrome_CheckedChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.AutoCloseDriver = reopenChrome.Checked;
        }

        private void autoRandomTasks_CheckedChanged(object sender, EventArgs e)
        {
            getSelectedAcc().Settings.AutoRandomTasks = autoRandomTasks.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getSelectedAcc().TaskTimer.Start();
            UpdateBotRunning();
        }
        public void UpdateBotRunning(string running = null)
        {
            if(string.IsNullOrEmpty(running)) running = getSelectedAcc()?.TaskTimer?.IsBotRunning()?.ToString();
            botRunning.Text = "Bot running: " + (string.IsNullOrEmpty(running) ? "false" : running);
        }
    }
}