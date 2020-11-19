using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TbsCore.Helpers;

namespace TravBotSharp.Views
{
    public partial class DebugUc : UserControl
    {
        private ControlPanel main;

        public DebugUc()
        {
            InitializeComponent();
        }
        public void Init(ControlPanel main)
        {
            this.main = main;
            Utils.LoggerSink.NewLogHandler += TbsLogger_NewLogHandler;
        }

        private void TbsLogger_NewLogHandler(object sender, EventArgs e)
        {
            var log = ((LogEventArgs)e).Log;

            var previousLogs = this.logTextBox.Text;
            // Max 10k chars of log (performance)
            if (10000 < previousLogs.Length) previousLogs = previousLogs.Substring(0, 10000);

            this.logTextBox.Text =
                $"{log.Timestamp.DateTime.ToString("HH:mm:ss")}: " +
                $"{log.MessageTemplate}\n{previousLogs}"; 
        }

        public void UpdateTab()
        {
            var acc = main.GetSelectedAcc();
            taskListView.Items.Clear();
            if (acc.Tasks == null) return;
            foreach(var task in acc.Tasks.ToList())
            {
                var item = new ListViewItem();
                item.SubItems[0].Text = task.ToString().Split('.').Last(); // Task name
                item.SubItems.Add(task.Vill?.Name ?? "/"); // Village name
                item.SubItems.Add(task.Priority.ToString());
                item.SubItems.Add(task.Stage.ToString());
                item.SubItems.Add(task.ExecuteAt.ToString());
                item.SubItems.Add(task.Message ?? "");
                taskListView.Items.Add(item);
            }
        }
    }
}
