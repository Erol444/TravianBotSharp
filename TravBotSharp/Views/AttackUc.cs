using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.AttackModels;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Views
{
    public partial class AttackUc : UserControl
    {
        ControlPanel main;
        List<List<SendWaveModel>> sendWaves = new List<List<SendWaveModel>>();
        public AttackUc()
        {
            InitializeComponent();
            // Time picker only
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.ShowUpDown = true;
        }
        public void Init(ControlPanel _main)
        {
            main = _main;
        }
        private Account getSelectedAcc()
        {
            return main != null ? main.GetSelectedAcc() : null;
        }
        public Village getSelectedVillage(Account acc = null)
        {
            return main != null ? main.GetSelectedVillage(acc) : null;
        }
        public void UpdateTab()
        {
            var acc = getSelectedAcc();

            richTextBox1.Text = "";
            foreach (var sendWave in sendWaves)
            {
                foreach (var attk in sendWave)
                {
                    var coords = $"({attk.Coordinates.x}/{attk.Coordinates.y}) ";
                    var type = GetWaveType(attk);
                    richTextBox1.AppendText(coords + type + " - " + attk.Arrival + "\n");
                }
            }
            dateTimePicker1.Value = DateTime.Now;

            currentlyBuildinglistView.Items.Clear();
            if (acc.Tasks == null) return;
            var sendWaveTasks = acc.Tasks.Where(x => x.GetType() == typeof(SendWaves));
            if (sendWaveTasks == null) return;
            foreach (var sendWaveTask in sendWaveTasks)
            {
                var task = (SendWaves)sendWaveTask;
                var item = new ListViewItem();
                var firstWave = task.SendWaveModels.FirstOrDefault();

                item.SubItems[0].Text = sendWaveTask.Vill.Name; // name of the village you are sending from
                item.SubItems.Add(firstWave.Coordinates.x + "/" + firstWave.Coordinates.y); //target coordinates
                item.SubItems.Add(GetWaveType(firstWave)); // type of the wave
                item.SubItems.Add(task.ExecuteAt.ToString()); // execute at
                item.SubItems.Add(firstWave.Arrival.ToString()); // arrive at
                currentlyBuildinglistView.Items.Add(item);
            }
        }

        private void confirmNewVill_Click(object sender, EventArgs e)
        {
            var coords = new Coordinates() { x = (int)X.Value, y = (int)Y.Value };
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
                    attk.AllOff = true;
                    attk.Arrival = firstWave;
                    attk.Troops[10] = hero.Checked ? 1 : 0;
                }
                else attk.DelayMs = (int)(1000 / perSec);

                attk.Coordinates = coords;
                attk.MovementType = Classificator.MovementType.Attack;
                attk.Troops[7] = catas;
                attacks.Add(attk);
            }
            sendWaves.Add(attacks);
            UpdateTab();
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
                    Vill = getSelectedVillage(),
                    SendWaveModels = wave.ToList(),
                    Priority = Files.Tasks.BotTask.TaskPriority.High
                };
                TaskExecutor.AddTask(getSelectedAcc(), waveTask);
            }
            sendWaves.Clear();
            UpdateTab();
        }

        private void button2_Click(object sender, EventArgs e) // Send fake
        {
            var coords = new Coordinates() { x = (int)X.Value, y = (int)Y.Value };
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
                attk.Coordinates = coords;
                attk.MovementType = Classificator.MovementType.Attack;
                attk.Troops[7] = 1;
                attacks.Add(attk);
            }
            sendWaves.Add(attacks);
            UpdateTab();
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
            string type = "";
            if (attk.FakeAttack) type = "Fake attack";
            else if (attk.AllOff) type = "Real attack";
            else type = "Catas";
            return type;
        }
    }
}