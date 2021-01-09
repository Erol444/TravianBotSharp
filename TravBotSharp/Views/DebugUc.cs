using System;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Models.AccModels;
using TravBotSharp.Interfaces;
using static TravBotSharp.Files.Models.AccModels.WebBrowserInfo;

namespace TravBotSharp.Views
{
    public partial class DebugUc : TbsBaseUc, ITbsUc
    {
        public DebugUc()
        {
            InitializeComponent();
        }

        // For thread safety
        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;
            return true;
        }
        public void NewLogHandler(object sender, EventArgs e)
        {
            var newLog = ((LogEventArgs)e).Log;
            if (ControlInvokeRequired(this.main, () => NewLogHandler(sender, e))) return;
            logTextBox.Text = newLog + "\n" + logTextBox.Text;
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();

            taskListView.Items.Clear();
            logTextBox.Clear();

            if (acc.Tasks == null) return;
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


            //new Thread(() => IoHelperCore.Logout(GetSelectedAcc())).Start();
            foreach (var log in acc.Wb.Logs)
            {
                logTextBox.AppendText(log + "\n");
            }

            this.Focus();
        }

        private void DebugUc_Enter(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            if (WbAvailable(acc)) acc.Wb.LogHandler += NewLogHandler;
        }
        private void DebugUc_Leave(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            if (WbAvailable(acc)) acc.Wb.LogHandler -= NewLogHandler;
        }
        private bool WbAvailable(Account acc) => acc?.Wb != null;
    }
}
