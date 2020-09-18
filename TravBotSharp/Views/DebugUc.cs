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
                taskListView.Items.Add(item);
            }
        }
    }
}
