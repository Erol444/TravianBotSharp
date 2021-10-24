using System;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TravBotSharp.Interfaces;
using static TbsCore.Models.AccModels.WebBrowserInfo;
using TbsCore.Models.Logging;

namespace TravBotSharp.Views
{
    public partial class DebugUc : TbsBaseUc, ITbsUc
    {
        public LogOutput Log;
        public bool active;

        public DebugUc()
        {
            InitializeComponent();
            active = false;
        }

        public void InitLog(LogOutput log)
        {
            Log = log;
            Log.LogUpdated += LogUpdate;
        }

        public void UpdateUc()
        {
            active = true;
            UpdateTaskTable();
            GetLogData();
            this.Focus();
        }

        public void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            active = false;
        }

        private void LogUpdate(object sender, UpdateLogEventArgs e)
        {
            if (!active) return;
            // only update current account
            if (e.Username == GetSelectedAcc().AccInfo.Nickname)
            {
                UpdateLogData();
            }
        }

        public void GetLogData()
        {
            if (!active) return;

            if (logTextBox.InvokeRequired)
            {
                logTextBox.BeginInvoke(new Action(delegate
                {
                    GetLogData();
                }));
                return;
            }
            logTextBox.Clear();
            var acc = GetSelectedAcc();
            logTextBox.Text = Log.GetLog(acc.AccInfo.Nickname);
        }

        public void UpdateLogData()
        {
            if (!active) return;
            if (logTextBox.InvokeRequired)
            {
                logTextBox.BeginInvoke(new Action(delegate
                {
                    UpdateLogData();
                }));
                return;
            }
            var acc = GetSelectedAcc();
            logTextBox.Text = $"{Log.GetLastLog(acc.AccInfo.Nickname)}{logTextBox.Text}";
        }

        public void UpdateTaskTable()
        {
            if (!active) return;
            if (taskListView.InvokeRequired)
            {
                taskListView.BeginInvoke(new Action(delegate
                {
                    UpdateTaskTable();
                }));
                return;
            }

            taskListView.Items.Clear();
            var acc = GetSelectedAcc();
            if (acc.Tasks != null)
            {
                foreach (var task in GetSelectedAcc().Tasks.ToList())
                {
                    var item = new ListViewItem();
                    item.SubItems[0].Text = task.ToString().Split('.').Last(); // Task name
                    item.SubItems.Add(task.Vill?.Name ?? "/"); // Village name
                    item.SubItems.Add(task.Priority.ToString());
                    item.SubItems.Add(task.Stage.ToString());
                    item.SubItems.Add(task.ExecuteAt.ToString());
                    taskListView.Items.Add(item);
                }
            }
        }
    }
}