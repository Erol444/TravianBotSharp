using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;
using TravBotSharp.Interfaces;
using TbsCore.Tasks.SecondLevel;

namespace TravBotSharp.Views
{
    public partial class AttackUc : BaseVillageUc, ITbsUc
    {
        private List<List<SendWaveModel>> sendWaves = new List<List<SendWaveModel>>();

        public AttackUc()
        {
            InitializeComponent();
            // Time picker only
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.ShowUpDown = true;
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();

            // Oasis farming controls
            oasisEnabled.Checked = vill.FarmingNonGold.OasisFarmingEnabled;
            oasisStrategy.SelectedIndex = (int)vill.FarmingNonGold.OasisFarmingType;
            if (vill.FarmingNonGold.OasisFarmingDelay == 0) vill.FarmingNonGold.OasisFarmingDelay = 5;
            oasisDelay.Value = vill.FarmingNonGold.OasisFarmingDelay;
            oasisDistance.Value = vill.FarmingNonGold.OasisMaxDistance;
            oasisPower.Value = vill.FarmingNonGold.MaxDeffPower;
            oasisMinTroops.Value = vill.FarmingNonGold.MinTroops;

            richTextBox1.Text = "";
            foreach (var sendWave in sendWaves)
            {
                foreach (var attk in sendWave)
                {
                    var coords = $"({attk.TargetCoordinates.x}/{attk.TargetCoordinates.y}) ";
                    var type = GetWaveType(attk);
                    richTextBox1.AppendText(coords + type + " - " + attk.Arrival + "\n");
                }
            }
            dateTimePicker1.Value = DateTime.Now;

            currentlyBuildinglistView.Items.Clear();
            if (acc.Tasks == null) return;
            var sendWaveTasks = acc.Tasks.FindTasks(typeof(SendWaves));
            if (sendWaveTasks == null) return;
            foreach (var sendWaveTask in sendWaveTasks)
            {
                var task = (SendWaves)sendWaveTask;
                var item = new ListViewItem();
                var firstWave = task.SendWaveModels.FirstOrDefault();

                item.SubItems[0].Text = sendWaveTask.Vill.Name; // name of the village you are sending from
                item.SubItems.Add(firstWave.TargetCoordinates.x + "/" + firstWave.TargetCoordinates.y); //target coordinates
                item.SubItems.Add(GetWaveType(firstWave)); // type of the wave
                item.SubItems.Add(task.ExecuteAt.ToString()); // execute at
                item.SubItems.Add(firstWave.Arrival.ToString()); // arrive at
                currentlyBuildinglistView.Items.Add(item);
            }
        }

        private void confirmNewVill_Click(object sender, EventArgs e)
        {
            var coords = coordinatesUc1.Coords;
            var numOfWaves = (int)WavesCount.Value;
            var perSec = (int)wavesPerSec.Value;
            var catas = (int)catasPerWave.Value;

            var firstWave = sendNow.Checked ? DateTime.Now.AddHours(-3) : GetArrival();
            var attacks = new List<SendWaveModel>();
            for (int i = 0; i < numOfWaves; i++)
            {
                var attk = new SendWaveModel();
                attk.Troops = new int[11];
                if (i == 0)
                {
                    attk.Troops = SendAllTroops();
                    attk.Arrival = firstWave;
                    attk.Troops[10] = hero.Checked ? 1 : 0;
                }
                else attk.DelayMs = (int)(1000 / perSec);

                attk.TargetCoordinates = coords;
                attk.MovementType = Classificator.MovementType.Attack;
                attk.Troops[7] = catas;
                attacks.Add(attk);
            }
            sendWaves.Add(attacks);
            UpdateUc();
        }

        /// <summary>
        ///  Populate the troops array with negative values - which means bot will send
        ///  all available units of that type
        /// </summary>
        private int[] SendAllTroops()
        {
            var ret = new int[11];
            var acc = GetSelectedAcc();
            for (int i = 0; i < 10; i++)
            {
                if (TroopsData.IsTroopOffensive(acc, i) || i == 6 /* Rams */)
                {
                    ret[i] = -1;
                }
            }
            return ret;
        }

        private void sendNow_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !sendNow.Checked;
            if (sendNow.Checked) dateTimePicker1.Value = DateTime.Today;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var wave in sendWaves)
            {
                var waveTask = new SendWaves()
                {
                    ExecuteAt = DateTime.Now.AddHours(-100), // Execute now, on we will create a correct ExecuteAt later
                    Vill = GetSelectedVillage(),
                    SendWaveModels = wave.ToList(),
                    Priority = TbsCore.Tasks.BotTask.TaskPriority.High
                };
                GetSelectedAcc().Tasks.Add(waveTask);
            }
            sendWaves.Clear();
            UpdateUc();
        }

        private void button2_Click(object sender, EventArgs e) // Send fake
        {
            var coords = coordinatesUc1.Coords;
            var numOfWaves = (int)WavesCount.Value;
            var perSec = (int)wavesPerSec.Value;

            var firstWave = sendNow.Checked ? DateTime.Now.AddHours(-3) : GetArrival();

            var attacks = new List<SendWaveModel>();
            for (int i = 0; i < numOfWaves; i++)
            {
                var attk = new SendWaveModel();
                attk.Troops = new int[11];
                if (i == 0)
                {
                    attk.Arrival = firstWave;
                }
                else attk.DelayMs = (int)(1000 / perSec);

                attk.FakeAttack = true;
                attk.TargetCoordinates = coords;
                attk.MovementType = Classificator.MovementType.Attack;
                attk.Troops[7] = 1;
                attacks.Add(attk);
            }
            sendWaves.Add(attacks);
            UpdateUc();
        }

        private DateTime GetArrival()
        {
            var timeOfDay = dateTimePicker1.Value.TimeOfDay;

            //if(DateTime.Now.TimeOfDay.Subtract(new TimeSpan(12,0,0)) > timeOfDay)
            //{
            //    return DateTime.Today.AddDays(1).Add(timeOfDay);
            //}
            return DateTime.Today.Add(timeOfDay);
        }

        private string GetWaveType(SendWaveModel attk)
        {
            if (attk.FakeAttack) return "Fake attack";
            else if (attk.Troops.Any(x => x < 0)) return "Real attack";
            else return "Catas";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().Tasks.Add(new AttackOasis());
        }

        #region Oasis farming callbacks

        private void oasisEnabled_CheckedChanged(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            vill.FarmingNonGold.OasisFarmingEnabled = oasisEnabled.Checked;

            if (oasisEnabled.Checked)
            {
                acc.Tasks.Add(new AttackOasis() { Vill = vill }, true, vill);
            }
            else // Remove all AttackOasis tasks for this village
            {
                acc.Tasks.Remove(typeof(AttackOasis), vill);
            }
        }

        private void oasisStrategy_SelectedIndexChanged(object sender, EventArgs e) =>
            GetSelectedVillage().FarmingNonGold.OasisFarmingType = (OasisFarmingType)oasisStrategy.SelectedIndex;

        private void oasisDelay_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().FarmingNonGold.OasisFarmingDelay = (int)oasisDelay.Value;

        private void oasisDistance_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().FarmingNonGold.OasisMaxDistance = (int)oasisDistance.Value;

        private void oasisPower_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().FarmingNonGold.MaxDeffPower = (int)oasisPower.Value;

        private void oasisMinTroops_ValueChanged(object sender, EventArgs e) =>
            GetSelectedVillage().FarmingNonGold.MinTroops = (int)oasisMinTroops.Value;

        #endregion Oasis farming callbacks


        // Scout player
        private void button3_Click_1(object sender, EventArgs e)
        {
            GetSelectedAcc().Tasks.Add(new ScoutPlayer()
            {
                UserId = (int)playerId.Value,
                Vill = GetSelectedVillage()
            });
        }
    }
}