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

        public DebugUc()
        {
            InitializeComponent();
        }

        public void InitLog(LogOutput log)
        {
            Log = log;
            Log.LogUpdated += LogUpdate;
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();

            taskListView.Items.Clear();
            logTextBox.Clear();
            GetLogData();
            this.Focus();
        }

        private void LogUpdate(object sender, TbsCore.Models.Logging.UpdateLogEventArgs e)
        {
            // only update current account
            if (e.Username == GetSelectedAcc().AccInfo.Nickname)
            {
                UpdateLogData();
            }
        }

        public void GetLogData()
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.BeginInvoke(new Action(delegate
                {
                    GetLogData();
                }));
                return;
            }

            var acc = GetSelectedAcc();
            logTextBox.Text = Log.GetLog(acc.AccInfo.Nickname);
        }

        public void UpdateLogData()
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.BeginInvoke(new Action(delegate
                {
                    GetLogData();
                }));
                return;
            }

            var acc = GetSelectedAcc();
            logTextBox.Text = $"{Log.GetLog(acc.AccInfo.Nickname)}{logTextBox.Text}";
        }

        private void UpdateTaskTable(Account acc)
        {
            if (acc.Tasks == null) return;
            taskListView.Items.Clear();
            foreach (var task in acc.Tasks.ToList())
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